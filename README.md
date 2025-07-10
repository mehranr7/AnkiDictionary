<h1>Anki Gemini Integration</h1>
<p>This application integrates with Anki using the Anki add-on <a href="https://ankiweb.net/shared/info/2055492159">AnkiConnect</a>. It retrieves information via the Gemini API and adds notes to Anki automatically.</p>
<h2>Requirements</h2>
<ul>
    <li><a href="https://ankiweb.net/shared/info/2055492159">AnkiConnect</a> must be installed in Anki.</li>
    <li>A Gemini API key is required to fetch data.</li>
</ul>

<h2>Features</h2>
<ul>
    <li>Extracts necessary fields from Gemini API responses.</li>
    <li>Adds notes to Anki via AnkiConnect.</li>
    <li>Configurable options in <code>appsettings.json</code>.</li>
</ul>

<h2>Configuration</h2>
<p>Modify the <code>appsettings.json</code> file to suit your preferences.</p>

<h3>Example <code>appsettings.json</code></h3>
<div>
<pre>{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY",
    "RegularAnswerCount": 10,
    "MaximumRequests": 10,
    "CoolDown": 120,
    "DefaultIntroduction": "I want you to function as a dictionary. The official reference sources for generating content are the Cambridge Dictionary and the Collins Dictionary. I will provide a text, and you will return the following details in a structured format:\n\n- Front: The exact text of the input provided.\n- Back: A simplified definition of the input text, avoiding the use of the original word or any of its word family.\n- Tags: Relevant tags to categorize and distinguish the term appropriately.\n\nEnsure that the Back section does not contain the original word or any of its word forms. The response must be structured as a JSON array.\n\nAdditionally, if I provide multiple words or phrases separated by commas, each should be treated as a distinct entry, generating multiple JSON objects within the array.\n\nLet's begin with the following words: Creatively, Effect."
  },
  "Speed": {
    "ShortPause": 150,
    "LongPause": 500
  },
  "General": {
    "UseAnkiConnect": true,
    "DeckName": "default",
    "ModelName": "Basic",
    "NewTag": "!New"
  },
  "DynamicObject": {
    "MainField": "Front",
    "Object": {
      "Front": "Essential",
      "Back": "Essential",
      "Tags": "Optional"
    }
  }
}</pre>
</div>

<h2>Explanation of Configuration</h2>
<ul>
    <li><strong>Gemini</strong>
        <ul>
            <li><code>ApiKey</code>: Your Gemini API key.</li>
            <li><code>RegularAnswerCount</code>: Number of notes requested at a time.</li>
            <li><code>MaximumRequests</code>: Max requests before cooldown.</li>
            <li><code>CoolDown</code>: Wait time (seconds) before making new requests.</li>
            <li><code>DefaultIntroduction</code>: Instructions for Gemini API to generate notes.</li>
        </ul>
    </li>
    <li><strong>Speed</strong>
        <ul>
            <li><code>ShortPause</code>: Time delay for small actions (ms).</li>
            <li><code>LongPause</code>: Time delay for longer actions (ms).</li>
        </ul>
    </li>
    <li><strong>General</strong>
        <ul>
            <li><code>UseAnkiConnect</code>: If <code>true</code>, uses AnkiConnect; if <code>false</code>, simulates keyboard input.</li>
            <li><code>DeckName</code>: Target deck for the notes.</li>
            <li><code>ModelName</code>: Note type used in Anki.</li>
            <li><code>NewTag</code>: Default tag for new notes.</li>
        </ul>
    </li>
    <li><strong>DynamicObject</strong>
        <ul>
            <li><code>MainField</code>: Primary field name.</li>
            <li><code>Object</code>: Defines required and optional fields for Anki notes.</li>
        </ul>
    </li>
</ul>

<h2>Usage</h2>
<ol>
    <li>Ensure Anki is running with AnkiConnect enabled.</li>
    <li>Set up <code>appsettings.json</code> with your Gemini API key.</li>
    <li>Run the application.</li>
    <li>Notes will be added automatically to Anki.</li>
</ol>

<h2>Report bugs or further recommendation</h2>
<p>Feel free to suggest your ideas or report bugs via <code>mr.mehran75@gmail.com</code></p>
