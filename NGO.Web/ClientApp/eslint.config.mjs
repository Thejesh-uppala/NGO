import globals from "globals";
import pluginJs from "@eslint/js";
import tseslintPlugin from "@typescript-eslint/eslint-plugin";
import tsParser from "@typescript-eslint/parser";
import importPlugin from "eslint-plugin-import";
import prettierPlugin from "eslint-plugin-prettier";

/** @type {import("eslint").Linter.Config[]} */
export default [
  {
    files: ["**/*.{js,mjs,cjs,ts}"],
    languageOptions: {
      parser: tsParser,
      sourceType: "module",
      ecmaVersion: "latest",
      globals: globals.browser,
    },
    plugins: {
      "@typescript-eslint": tseslintPlugin,
      "import": importPlugin,
      "prettier": prettierPlugin, // Explicitly use the Prettier plugin
    },
    rules: {
      "prettier/prettier": "error", // Fix: Use the plugin instead of eslint-config-prettier
      "import/order": ["error", { "alphabetize": { "order": "asc" } }],
      "@typescript-eslint/no-unused-vars": ["error", { "argsIgnorePattern": "^_" }],
      "no-console": "warn", // Warn on console.log
    },
  },
  pluginJs.configs.recommended, // ESLint JavaScript recommendations
];
