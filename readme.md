## Chat with your SQL Data

This is the demo app for the youtube video: https://www.youtube.com/watch?v=hw6oTjw9_Ro

The app uses AI to generate queries for a relational SQL database based on natural language, meaning there is no requirement for an indexing search service, vector queries, or other usual AI architectures.

The AI service does NOT index any of your data, it just needs the basic table schema to understand how your database is structured. It generates queries based on these relationships.

Important: This app works best with small to medium sized databases with clear table relationships based on standard primary key and foreign key relationships with clear naming.

There is no troubleshooting support provided, feel free to fork the app and use for your own purposes.

## Setup steps

1. In the `SchemaLoader` project, replace the connection string placeholder with your own database connection, then run the app to generate your schema. Copy the schema into a text file for later use.

1. In the `YourOwnData` project, in the DataService.cs file, replace the connection string placeholder with your own database connection - the same connection string as in step 1.

1. In the `Index.cshtml.cs` file, replace the YOUR_SCHEMA placeholder in the AI prompt instructions section with the schema you copied previously.

1. In the `Index.cshtml.cs` file again, replace the OpenAI placeholder values towards the top so the app can connect to your AI service.













