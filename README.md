The application works with an Anki add-on named AnkiConnect (https://ankiweb.net/shared/info/2055492159).
It is required to have a Gemini API key to be able to get information from.
The application gives the fields required from Gemini and adds them into Anki using AnkiConnect.
There are some options that have to be configured in the appsettings.json file.
You can configure your settings using the information below:
{
  "Gemini": {
    // The Gemini API key that you should get from its website 
    "ApiKey": "YOUR_GEMINI_API_KEY",
    // The count of notes will be asked from Gemini when you provide multiple items  
    "RegularAnswerCount" : 10,
    // The maximum number of request sent before cool down
    "MaximumRequests" : 10,
    // The time to cool down before asking again
    "CoolDown" : 120,
    // a default introduction for starting the conversation with Gemini. This is the most important part, so try to explain all the fields Gemini should provide to you as a JSON object.
    "DefaultIntroduction": "I want you to act as a dictionary. The official resources to generate the content are the Cambridge Dictionary (https://dictionary.cambridge.org/) and the Collins Dictionary (https://www.collinsdictionary.com/). I'll provide a text. give me the following things: Front: the exact text of the provided text. Back: definition of the provided text in a simple way without using the provided text or its word family. Tags : the tags related to the provided text to categorize and differentiate items. Consider not to use the exact of the provided text or any word family of the provided text in the Back part. Consider the Whole answer must be in the form of a JSON array. Consider when I separate a word or a group of words using a comma the answer must be multiple JSON in an array. For example, let's start with these words : Creatively, Effect"
  },
  // The delay while the app is simulating the keyboard to add notes.
  "Speed": {
    "ShortPause": 150,  
    "LongPause": 500  
  },
  "General":  {
    // If the value of UseAnkiConnect is set to 'true', the application will use AnkiConnect to add cards. When it is set to 'false' the application will simulate the keyboard to add notes.
    "UseAnkiConnect": true,
    // the name of the destined deck
    "DeckName" : "default",
    // the name of note type you are using for your deck. You can change note types in Anki from Tools > Manage Note Types 
    "ModelName" : "Basic",
    // The tag that is assigned to new notes
    "NewTag" : "!New"
  },
  // You can customize fields for your note. This item should be similar to the note type your deck is based on.
  "DynamicObject": 
    {
      // the main field. do not change the name of this field
      "MainField" : "Front",
      // The object is your note's form. do not change the name of this field
      "Object" : {
        // All of the fields in your note type have to be written below. These fields have to be asked from Gemini. You can mention each of the fields below in "DefaultIntroduction" section.
        // if essential fields are not found in the answer given by Gemini, the note will be skipped. However, if optional fields are not found, it does not stop the process. 
        "Front" : "Essential",
        "Back" : "Essential",
        // use the item below for getting tags from Gemini (you should mention this field in "DefaultIntroduction").
        "Tags" : "Optional"
      }
  }
}
