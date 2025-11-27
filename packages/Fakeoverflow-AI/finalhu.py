import lmstudio as lms
import requests 
import os
from datetime import datetime

model = lms.llm(
"qwen/qwen3-vl-4b"
, config={"contextLength": 4000}
)

claim_text = "Bitcoin has gone to 50k usd"
print(claim_text)

search_api_url = "https://serpapi.com/search"  
search_api_key = os.getenv("SERPAPI_API_KEY")
current_date = datetime.now().strftime("%Y-%m-%d")

lowConfig = {
"temperature": 0.1,
"topKSampling": 1,
"maxTokens": 500
}
def generateQueries(claim_text: str):

    chat = lms.Chat("/no_think")
    chat.add_user_message(
                        "You are a web search expert \n"
                        f"Today's date is: {current_date}\n"
                        f"create 3 to 4 relevant web search queries about the input text {claim_text}.\n"
                        "Based on the input claim, generate 3-4 relevant web search queries without assuming any additional external knowledge.\n"  
                        "Each query shall be a single sentence, and each in separate lines, do not include any other data.\n"
                        "At least one query shall be with the current date.\n"
                        "If the claim mentions a historical event, generate at least one query with that events date.\n")
    
    response = model.respond(chat, config=lowConfig)
    print(response.content)
    queries = [q.strip() for q in response.content.split("\n") if q.strip()]
    return queries

def searchWeb(query: str):
    params = {
        "q": query,
        "api_key": search_api_key,
        "engine": "google"
    }
    try:
        response = requests.get(search_api_url, params=params)
        response.raise_for_status()
        return response.json()
    except Exception as e:
        print(f"Error searching for '{query}': {e}")
        return None

def extractWebResults(web_results):
    """Extract relevant information from search results"""
    if not web_results:
        return "No results found."

    
    results_text = []
    
    if "organic_results" in web_results:
        for i, result in enumerate(web_results["organic_results"][:5], 1):
            title = result.get("title", "")
            snippet = result.get("snippet", "")
            results_text.append(f"Result {i}: {title}\n{snippet}")
    
    if "answer_box" in web_results:
        answer = web_results["answer_box"].get("answer", "")
        if answer:
            results_text.insert(0, f"Featured Answer: {answer}")
    
    return "\n\n".join(results_text) if results_text else "No relevant results found."
queries = generateQueries(claim_text)

all_results_blocks = []
for q in queries:
    print(f"\n Searching for: {q} ")
    data = searchWeb(q)
    text_block = extractWebResults(data)
    all_results_blocks.append(f"### Results for query: {q}\n{text_block}")


web_results = "\n\n".join(all_results_blocks)
print("reason:")

highConfig = {
"temperature": 0.7,
"topKSampling": 10,
"maxTokens": 500
}
chat = lms.Chat("/no_think")
chat.add_user_message(f"original claim: {claim_text}\n"
f"web search results: {web_results}\n"
f"Today's date is: {current_date}\n"
f"Based on the original claim and web search results, Understand and reason if the statement is likely true or likely false or uncertain.\n"
"You are a fact analyzer \n"
"interpret that data and the original user claim, and reason if the user claim aligns with the web response data. \n"
"Notice and check for negation statements in the claim, such as 'not', 'didn't', etc. \n"
"based on the data, alignment of user claim and web response assign a mathematical score, name it 'Alignment Score' \n"

"if 75% <= Alignment Score, respond: Most likely True. \n"

"if 60% <= Alignment Score < 75%, respond Likely True. \n"

"If 40% <= Alignment Score < 60%, respond: Uncertain. \n"

"If 25% <= Alignment Score< 40%,  respond: Likely False. \n"

"If Alignment Score < 25%, respond: Most Likely False. \n"

)
response = model.respond(chat, config=highConfig)
print(response.content)

