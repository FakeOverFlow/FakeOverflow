const readline = await import('readline');
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});
const askQuestion = (question) => {
    return new Promise((resolve) => {
        rl.question(question, (answer) => {
            resolve(answer);
        });
    });
};

function validateSwaggerUrl(url) {
    if(!url || url.trim() === "") return false;

    return url.startsWith("http://") || url.startsWith("https://");
}

// START LOGIC FROM HERE - I JUST PUT THE ABOVE PART FOR EASE OF USE

const GENERATOR_URL = "http://api.openapi-generator.tech/";

const main = async () => {
    let url = await askQuestion("Enter the generator URL (Pass empty to use default): ");
    if(!url || url.trim() === "") {
        url = GENERATOR_URL;
    }

    if(!url.endsWith("/")) {
        url = url + "/";
    }

    console.log("URL has been set to: " + url);

    url = `${url}api/gen/clients/typescript-angular`;
    console.log("The generation url has been set to: " + url);

    let swaggerUrl = undefined;
    while (validateSwaggerUrl(swaggerUrl) === false) {
        swaggerUrl = await askQuestion("Enter the swagger JSON URL: ");
    }

    console.log("The swagger URL has been set to: " + swaggerUrl);

    const body = {
        options: {
            ngVersion: "20",
        },
        openAPIUrl: swaggerUrl
    }

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    if(!response.ok) {
        console.error("Failed to generate the service. Error: " + response.statusText);
        console.warn("Please check the swagger URL and try again.");
    }

    console.log("Service generated successfully!");
    const arrayBuffer = await response.arrayBuffer();
    const buffer = Buffer.from(arrayBuffer);

    const outputDir = path.resolve(__dirname, "../packages/fakeoverflow-angular-services");
    fs.mkdirSync(outputDir, { recursive: true });

    const outputFile = path.join(outputDir, "fakeoverflow-angular-services.zip");
    fs.writeFileSync(outputFile, buffer);

    console.log(`Saved generated service to: ${outputFile}`);
};

await main();


