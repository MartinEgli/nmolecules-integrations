'use strict';

const path = require('node:path');

async function main() {
  const { runTests } = require('@vscode/test-electron');
  const extensionDevelopmentPath = path.resolve(__dirname, '..');
  const extensionTestsPath = path.resolve(__dirname, 'suite', 'index.js');

  await runTests({
    extensionDevelopmentPath,
    extensionTestsPath
  });
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});
