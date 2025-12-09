# Auto-Implementation Scripts

This directory contains scripts for AI-powered automatic issue implementation.

## ai-implement.js

This script is called by the auto-pr-on-issue workflow to attempt automatic code implementation using AI.

### Current Status

The script is currently a **template** that prepares the groundwork for AI integration but doesn't implement code by itself. It serves as a starting point for custom AI integration.

### What It Does Now

1. ✅ Checks for AI API key configuration
2. ✅ Reads issue context and task files
3. ✅ Provides integration guidance
4. ✅ Directs to GitHub Copilot Workspace for immediate implementation

### How to Enable Full AI Implementation

To enable automatic code generation and implementation, you need to extend this script with actual AI API integration:

#### Option 1: OpenAI Integration

1. Add `OPENAI_API_KEY` to your repository secrets
2. Extend the `generateCodeWithAI()` function to call OpenAI's API:

```javascript
async function generateCodeWithAI({ task, context, issue }) {
  const { Configuration, OpenAIApi } = require('openai');
  
  const configuration = new Configuration({
    apiKey: config.openaiApiKey,
  });
  const openai = new OpenAIApi(configuration);

  const prompt = `
You are a code implementation assistant. 

Repository Context:
${context}

Task:
${task}

Issue:
${issue}

Generate the necessary code changes to implement this issue.
`;

  const response = await openai.createChatCompletion({
    model: 'gpt-4',
    messages: [{ role: 'user', content: prompt }],
  });

  return response.data.choices[0].message.content;
}
```

3. Install required dependency: `npm install openai`

#### Option 2: Anthropic Claude Integration

1. Add `ANTHROPIC_API_KEY` to your repository secrets
2. Extend the `generateCodeWithAI()` function to call Anthropic's API:

```javascript
async function generateCodeWithAI({ task, context, issue }) {
  const Anthropic = require('@anthropic-ai/sdk');
  
  const anthropic = new Anthropic({
    apiKey: config.anthropicApiKey,
  });

  const prompt = `
You are a code implementation assistant.

Repository Context:
${context}

Task:
${task}

Issue:
${issue}

Generate the necessary code changes to implement this issue.
`;

  const response = await anthropic.messages.create({
    model: 'claude-3-opus-20240229',
    max_tokens: 4000,
    messages: [{ role: 'user', content: prompt }],
  });

  return response.content[0].text;
}
```

3. Install required dependency: `npm install @anthropic-ai/sdk`

#### Option 3: GitHub Copilot Workspace (Recommended - No Code Required)

The easiest and most integrated solution is to use GitHub Copilot Workspace:

1. **No API keys needed** - uses your GitHub Copilot subscription
2. **No code changes needed** - works out of the box
3. **Automatic implementation** - just click the link in the PR

When an auto-fix PR is created:
1. Open the Copilot Workspace link in the PR description
2. Copilot analyzes the issue and codebase
3. Proposes implementation
4. Commits code directly to the PR

**This is the recommended approach** and requires no additional configuration.

### Implementing Code Changes

After generating code with AI, you need to implement the `applyImplementation()` function to:

1. Parse the AI-generated code
2. Write changes to the appropriate files
3. Create git commits
4. Push to the PR branch

Example:

```javascript
async function applyImplementation(implementation) {
  const { execSync } = require('child_process');
  
  // Parse the implementation (this depends on your AI response format)
  const changes = parseImplementation(implementation);
  
  // Apply each change
  for (const change of changes) {
    fs.writeFileSync(change.file, change.content);
  }
  
  // Commit changes
  execSync('git config user.name "Auto-Fix Bot"');
  execSync('git config user.email "auto-fix@github.com"');
  execSync('git add .');
  execSync(`git commit -m "Auto-implement: ${config.issueTitle}"`);
  execSync('git push');
  
  return true;
}
```

### Testing

To test the script locally:

```bash
export ISSUE_NUMBER=1
export ISSUE_TITLE="Test issue"
export ISSUE_BODY="Test description"
export GITHUB_REPOSITORY="owner/repo"
export OPENAI_API_KEY="your-key" # optional

node .github/scripts/ai-implement.js
```

### Dependencies

If you extend this script with AI APIs, add a `package.json` in `.github/scripts/`:

```json
{
  "name": "auto-fix-scripts",
  "version": "1.0.0",
  "dependencies": {
    "openai": "^4.0.0",
    "@anthropic-ai/sdk": "^0.9.0"
  }
}
```

Then update the workflow to install dependencies before running the script:

```yaml
- name: Install script dependencies
  run: |
    cd .github/scripts
    npm install
```

## Contributing

Contributions to enhance the AI implementation capabilities are welcome! Please ensure:

1. The script remains backward compatible
2. AI API keys are always optional
3. Fallback to Copilot Workspace is maintained
4. Error handling is robust

## Questions?

See the [Implementation Guide](.github/auto-fix/IMPLEMENTATION-GUIDE.md) for more details.
