import readline from 'readline';
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

/**
 * Updates the generated package.json to be compatible with the pnpm monorepo.
 * It adds 'main', 'module', 'types', 'exports', and 'files' fields.
 * @param {string} packageDir The directory of the generated package.
 */
const updatePackageJson = async (packageDir) => {
    console.log('📄 Patching package.json for monorepo compatibility...');

    const packageJsonPath = path.join(packageDir, 'package.json');

    if (!fs.existsSync(packageJsonPath)) {
        console.error('❌ package.json not found in the output directory. Skipping patch.');
        return;
    }

    // Read and parse the existing package.json
    const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf-8'));
    const packageName = packageJson.name; // Get the package name (e.g., 'fakeoverflow-angular-services')

    if (!packageName) {
        console.error('❌ Could not determine package name from package.json. Skipping patch.');
        return;
    }

    // Add the necessary fields for monorepo tooling
    packageJson.main = `./dist/fesm2022/${packageName}.mjs`;
    packageJson.module = `./dist/fesm2022/${packageName}.mjs`;
    packageJson.types = `./dist/index.d.ts`;
    packageJson.files = ["dist"];
    packageJson.exports = {
        ".": {
            "types": "./dist/index.d.ts",
            "esm2022": `./dist/esm2022/${packageName}.mjs`,
            "es2022": `./dist/fesm2022/${packageName}.mjs`,
            "default": `./dist/fesm2022/${packageName}.mjs`
        }
    };

    fs.writeFileSync(packageJsonPath, JSON.stringify(packageJson, null, 2));

    console.log('✅ package.json patched successfully!');
};


const rootFiles = [];
const installablePackages = [
    {
        name: 'tslib',
        version: '2.8.1',
        dev: false
    },
    {
        name: 'typescript',
        version: '5.9.2',
        dev: true
    }
];

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

    const outputDir = path.resolve(__dirname, "../packages/fakeoverflow-angular-services/");

    try {
        console.log("🔥 Generating client with OpenAPI Generator CLI...");

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

        // ===================================================================
        // NEW STEP: Patch the package.json to make it monorepo compatible
        // ===================================================================
        await updatePackageJson(outputDir);


        rootFiles.forEach(file => {
            const filePath = path.join(outputDir, file);
            if (!fs.existsSync(filePath)) {
                console.warn(`${filePath} does not exist. Please check the output directory.`);
            }

            const rootDirectoryPath = path.join(outputDir, "../", file);
            console.log(`Root Directory Path: ${rootDirectoryPath}`)
            fs.copyFileSync(filePath, rootDirectoryPath);
            fs.unlinkSync(filePath);
        });

        console.log('📦 Installing dependencies for the new service...');
        installablePackages.forEach(pkg => {
            const command = `pnpm install --filter fakeoverflow-angular-services ${pkg.name}@${pkg.version} ${pkg.dev ? '--save-dev' : ''}`;
            console.log("Executing:", command);
            execSync(command, { stdio: 'inherit' });
        });

        console.log('📦 Installing root dependencies...');
        execSync(`pnpm install`, { stdio: 'inherit' });

        console.log('🚀 Building the new service package...');
        execSync(`pnpm --filter fakeoverflow-angular-services run build`, { stdio: 'inherit' });

        console.log('🎉 All done! Your new Angular service is ready to be used.');

    } catch (error) {
        console.error("❌ Failed to generate client:", error.message);
    } finally {
        rl.close();
    }
};

await main();