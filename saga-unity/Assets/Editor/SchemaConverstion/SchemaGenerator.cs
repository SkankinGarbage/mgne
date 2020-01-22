using UnityEngine;
using System.Collections;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System.CodeDom;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System;

namespace Mgne1 {

    public class SchemaGenerator {

        private const string SchemaDirectory = "SchemaJson";

        [MenuItem("MGNE Tools/Convert Schema")]
        public static void ConvertSchema() {
            GenerateSchema();
        }

        private static void GenerateSchema() {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace @namespace = new CodeNamespace("DBSchema");
            compileUnit.Namespaces.Add(@namespace);

            compileUnit.ReferencedAssemblies.AddRange(new string[] { "System", "System.Collections.Generic", "Unity.Engine" });

            Dictionary<CodeTypeDeclaration, string> generatedClasses = new Dictionary<CodeTypeDeclaration, string>();

            var directoryPath = Application.dataPath + "/" + SchemaDirectory;
            foreach (string filePath in Directory.GetFiles(directoryPath)) {
                if (!filePath.EndsWith(".json")) {
                    continue;
                }

                FileStream fileReader = File.Open(filePath, FileMode.Open);
                StreamReader streamReader = new StreamReader(fileReader);
                string jsonString = streamReader.ReadToEnd();
                SchemaClass schemaClassJson;
                try {
                    schemaClassJson = JsonConvert.DeserializeObject<SchemaClass>(jsonString, settings);
                } finally {
                    streamReader.Close();
                    fileReader.Close();
                }
                if (schemaClassJson == null) {
                    Debug.LogError("Invalid schema: " + filePath);
                    continue;
                }
                Debug.Log("Parsing " + filePath + "...");

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                string name = schemaClassJson.name;
                if (name.EndsWith("MDO")) {
                    name = name.Substring(0, name.IndexOf("MDO"));
                }
                CodeTypeDeclaration schemaClass = new CodeTypeDeclaration(name + "Data");

                if (!schemaClassJson.excludeFromTree) {
                    schemaClass.BaseTypes.Add(new CodeTypeReference("UnityEngine.ScriptableObject"));
                    string path = textInfo.ToTitleCase(schemaClassJson.path);
                    CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("UnityEngine.CreateAssetMenu",
                        new CodeAttributeArgument("fileName", new CodePrimitiveExpression(name)),
                        new CodeAttributeArgument("menuName", new CodePrimitiveExpression("Data/" + path)));
                    schemaClass.CustomAttributes.Add(attribute);
                } else {
                    schemaClass.BaseTypes.Add(new CodeTypeReference("AutoExpandingScriptableObject"));
                }

                foreach (SchemaField fieldJson in schemaClassJson.fields) {
                    if (fieldJson.name == "name" ||
                        fieldJson.name == "key" ||
                        fieldJson.name == "subfolder") {
                        continue;
                    }

                    CodeTypeReference type;
                    if (fieldJson.schemaLinkedClass == null) {
                        switch (fieldJson.type) {
                            case "String[]": type = new CodeTypeReference(typeof(List<>).MakeGenericType(typeof(string))); break;
                            case "String": type = new CodeTypeReference("string"); break;
                            case "Integer": type = new CodeTypeReference("int"); break;
                            case "Float": type = new CodeTypeReference("float"); break;
                            default: type = new CodeTypeReference("object"); break;
                        }
                    } else {
                        type = new CodeTypeReference(fieldJson.schemaLinkedClass);
                    }

                    CodeMemberField field = new CodeMemberField(type, fieldJson.name);
                    field.Attributes = MemberAttributes.Public;
                    schemaClass.Members.Add(field);

                    if (fieldJson.defaultValue != null && fieldJson.defaultValue.Length > 0) {
                        field.InitExpression = new CodePrimitiveExpression(fieldJson.defaultValue);
                    }

                    if (fieldJson.description != null && fieldJson.description.Length > 0) {
                        CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("UnityEngine.Tooltip",
                            new CodeAttributeArgument(new CodePrimitiveExpression(fieldJson.description)));
                        field.CustomAttributes.Add(attribute);
                    }

                    //if (fieldJson.header) {
                    //    CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("UnityEngine.Space",
                    //        new CodeAttributeArgument(new CodePrimitiveExpression(24)));
                    //    field.CustomAttributes.Add(attribute);
                    //}
                }

                generatedClasses[schemaClass] = name;
                @namespace.Types.Add(schemaClass);
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            foreach (var generatedClass in generatedClasses) {
                string outputFilePath = Application.dataPath + "/gen/" + generatedClass.Value + "Data.cs";
                using (StreamWriter sw = new StreamWriter(outputFilePath, false)) {
                    Debug.Log("Writing to " + outputFilePath + "...");
                    IndentedTextWriter writer = new IndentedTextWriter(sw, "    ");
                    provider.GenerateCodeFromType(generatedClass.Key, writer, new CodeGeneratorOptions());
                    writer.Close();
                }
            }
        }
    }
}
