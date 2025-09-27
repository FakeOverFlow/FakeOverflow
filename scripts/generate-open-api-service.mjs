const readline = await import('readline');
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { execSync } from "child_process";

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

// START LOGIC FROM HERE
const API_URL = "https://api-fof.alenalex.me/swagger/v1/swagger.json";

const main = async () => {
    let swaggerUrl = await askQuestion(`Enter the swagger JSON URL (default: ${API_URL}): `);
    if(!swaggerUrl || swaggerUrl.trim() === "") {
        swaggerUrl = API_URL;
    }

    if (!validateSwaggerUrl(swaggerUrl)) {
        console.error("❌ Invalid swagger URL provided");
        rl.close();
        return;
    }

    console.log("The swagger URL has been set to: " + swaggerUrl);

    const outputDir = path.resolve(__dirname, "../packages/fakeoverflow-angular-services");

    try {
        console.log("Generating client with OpenAPI Generator CLI...");

        // Create output directory
        fs.mkdirSync(outputDir, { recursive: true });

        // Build the command
        const command = [
            'npx @openapitools/openapi-generator-cli generate',
            `-i "${swaggerUrl}"`,
            '-g typescript-angular',
            `-o "${outputDir}"`,
            '--additional-properties ngVersion=20,npmName=fakeoverflow-angular-services,snapshot=false'
        ].join(' ');

        console.log("Executing:", command);
        execSync(command, { stdio: 'inherit' });

        console.log("✅ Client generated successfully!");
        console.log(`📁 Output directory: ${outputDir}`);

    } catch (error) {
        console.error("❌ Failed to generate client:", error.message);
    } finally {
        rl.close();
    }
};

await main();
