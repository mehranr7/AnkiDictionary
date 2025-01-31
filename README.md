The application works with an Anki add-on named AnkiConnect (https://ankiweb.net/shared/info/2055492159).<br>
It is required to have a Gemini API key to be able to get information from.
The application gives the fields required from Gemini and adds them into Anki using AnkiConnect.<br>
There are some options that have to be configured in the appsettings.json file.<br>
You can configure your settings using the information below:<br>
{<br>
  "Gemini": {<br>
    // The Gemini API key that you should get from its website <br>
    "ApiKey": "YOUR_GEMINI_API_KEY",<br>
    // The count of notes will be asked from Gemini when you provide multiple items  <br>
    "RegularAnswerCount" : 10,<br>
    // The maximum number of requests sent before cool down<br>
    "MaximumRequests" : 10,<br>
    // The time to cool down before asking again<br>
    "CoolDown" : 120,<br>
    // a default introduction for starting the conversation with Gemini. This is the most important part, so try to explain all the fields Gemini should provide to you as a JSON object.<br>
    "DefaultIntroduction": "I want you to act as a dictionary. The official resources to generate the content are the Cambridge Dictionary (https://dictionary.cambridge.org/) and the Collins Dictionary (https://www.collinsdictionary.com/). I'll provide a text. give me the following things: Front: the exact text of the provided text. Back: definition of the provided text in a simple way without using the provided text or its word family. Tags : the tags related to the provided text to categorize and differentiate items. Consider not to use the exact of the provided text or any word family of the provided text in the Back part. Consider the Whole answer must be in the form of a JSON array. Consider when I separate a word or a group of words using a comma the answer must be multiple JSON in an array. For example, let's start with these words : Creatively, Effect"<br>
  },<br>
  // The delay while the app is simulating the keyboard to add notes.<br>
  "Speed": {<br>
    "ShortPause": 150,  <br>
    "LongPause": 500  <br>
  },<br>
  "General":  {<br>
    // If the value of UseAnkiConnect is set to 'true', the application will use AnkiConnect to add cards. When it is set to 'false' the application will simulate the keyboard to add notes.<br>
    "UseAnkiConnect": true,<br>
    // the name of the destined deck<br>
    "DeckName" : "default",<br>
    // the name of note type you are using for your deck. You can change note types in Anki from Tools > Manage Note Types <br>
    "ModelName" : "Basic",<br>
    // The tag that is assigned to new notes<br>
    "NewTag" : "!New"<br>
  },<br>
  // You can customize fields for your note. This item should be similar to the note type your deck is based on.<br>
  "DynamicObject": <br>
    {<br>
      // the main field. do not change the name of this field<br>
      "MainField" : "Front",<br>
      // The object is your note's form. do not change the name of this field<br>
      "Object" : {<br>
        // All of the fields in your note type have to be written below. These fields have to be asked from Gemini. You can mention each of the fields below in "DefaultIntroduction" section.<br>
        // if essential fields are not found in the answer given by Gemini, the note will be skipped. However, if optional fields are not found, it does not stop the process. <br>
        "Front" : "Essential",<br>
        "Back" : "Essential",<br>
        // use the item below for getting tags from Gemini (you should mention this field in "DefaultIntroduction").<br>
        "Tags" : "Optional"<br>
      }
  }
}
