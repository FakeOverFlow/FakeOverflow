import sys
import json
import requests
import os
from datetime import datetime
from langchain_openai import ChatOpenAI


def main():
    # Parse arguments
    if len(sys.argv) < 7:
        error_response = {
            "success": False,
            "error": "Usage: python fact_checker.py <title> <content> <tags> <openai_api_key> <model_name> <api_base_url> <optional: serpapi_key>"
        }
        print(json.dumps(error_response))
        sys.exit(1)

    title = sys.argv[1]
    content = sys.argv[2]
    tags = sys.argv[3]
    openai_key = sys.argv[4]
    model_name = sys.argv[5]
    api_base_url = sys.argv[6]

    # SerpAPI key
    search_api_key = sys.argv[7] if len(sys.argv) > 7 else os.getenv("SERPAPI_API_KEY")
    if not search_api_key:
        error_response = {
            "success": False,
            "error": "SERPAPI_API_KEY not provided (CLI argument or environment variable)"
        }
        print(json.dumps(error_response))
        sys.exit(1)

    # Register OpenAI key and endpoint
    os.environ["OPENAI_API_KEY"] = openai_key
    os.environ["OPENAI_BASE_URL"] = api_base_url

    # Build claim text
    claim_text = f"{title}. {content}"
    if tags:
        claim_text += f" Tags: {tags}"

    try:
        # Initialize LLM
        llm = ChatOpenAI(
            model=model_name,
            base_url=api_base_url,
            api_key=openai_key,
            temperature=0.1
        )

        current_date = datetime.now().strftime("%Y-%m-%d")

        # Generate search queries
        queries = generate_queries(llm, claim_text, current_date)

        # Fetch SERPAPI data
        all_results_blocks = []
        for q in queries:
            data = search_web(q, search_api_key)
            text_block = extract_web_results(data)
            all_results_blocks.append(f"### Results for query: {q}\n{text_block}")

        web_results = "\n\n".join(all_results_blocks)

        # Analyze claim with higher-temperature model
        result = analyze_claim(llm, model_name, api_base_url, openai_key, claim_text, web_results, current_date)

        # Output JSON
        output = {
            "success": True,
            "claim": claim_text,
            "title": title,
            "content": content,
            "tags": tags,
            "queries": queries,
            "verdict": result,
            "timestamp": current_date,
            "model_used": model_name,
            "api_base_url": api_base_url
        }

        print(json.dumps(output))

    except Exception as e:
        print(json.dumps({"success": False, "error": str(e)}))
        sys.exit(1)


def generate_queries(llm, claim_text, current_date):
    prompt = f"""
You are a web search expert.
Today's date is: {current_date}

Create 3–4 relevant web search queries about:
{claim_text}

Rules:
- 3–4 queries
- Each on its own line
- Include today's date in one query
- If the claim mentions a historical event, include that date
"""
    response = llm.invoke(prompt).content
    queries = [q.strip() for q in response.split("\n") if q.strip()]
    return queries[:4]


def search_web(query, api_key):
    url = "https://serpapi.com/search"
    params = {
        "q": query,
        "api_key": api_key,
        "engine": "google"
    }
    try:
        res = requests.get(url, params=params)
        res.raise_for_status()
        return res.json()
    except:
        return None


def extract_web_results(web_results):
    if not web_results:
        return "No results found."

    results_text = []

    if "organic_results" in web_results:
        for i, r in enumerate(web_results["organic_results"][:5], 1):
            title = r.get("title", "")
            snippet = r.get("snippet", "")
            results_text.append(f"Result {i}: {title}\n{snippet}")

    if "answer_box" in web_results:
        ans = web_results["answer_box"].get("answer", "")
        if ans:
            results_text.insert(0, f"Featured Answer: {ans}")

    return "\n\n".join(results_text) if results_text else "No relevant results found."


def analyze_claim(base_llm, model_name, api_base_url, api_key, claim_text, web_results, current_date):
    reasoning_llm = ChatOpenAI(
        model=model_name,
        base_url=api_base_url,
        api_key=api_key,
        temperature=0.7
    )

    prompt = f"""
Original claim: {claim_text}

Web search results:
{web_results}

Today's date: {current_date}

Analyze whether the claim is true, false, or uncertain.
Assign an Alignment Score 0–100%.

Thresholds:
- ≥ 75% = Most likely True
- 60–74% = Likely True
- 40–59% = Uncertain
- 25–39% = Likely False
- < 25% = Most Likely False

Provide reasoning + final verdict.
"""
    return reasoning_llm.invoke(prompt).content


if __name__ == "__main__":
    main()
