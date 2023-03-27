using KdlDotNet;

namespace WebMagic
{
    public enum Style { Property, Attribute }
    public enum Type { Button, String, Number, Boolean, Date, Time, DateTime, List, Dictionary, Object, File, Image, Video, Audio, Email, Url, Phone, Color, Password, Text }
    public enum Required { True, False }
    

    public static class TypeMethods 
    {
        public static String GetString(this Type t1)
        {
            switch (t1)
            {
                case Type.Button:
                    return "Button";
                case Type.String:
                    return "String";
                case Type.Number:
                    return "Number";
                case Type.Boolean:
                    return "Boolean";
                case Type.Date:
                    return "Date";
                case Type.Time:
                    return "Time";
                case Type.DateTime:
                    return "DateTime";
                case Type.List:
                    return "List";
                case Type.Dictionary:
                    return "Dictionary";
                case Type.Object:
                    return "Object";
                case Type.File:
                    return "File";
                case Type.Image:
                    return "Image";
                case Type.Video:
                    return "Video";
                case Type.Audio:
                    return "Audio";
                case Type.Email:
                    return "Email";
                case Type.Url:
                    return "Url";
                case Type.Phone:
                    return "Phone";
                case Type.Color:
                    return "Color";
                case Type.Password:
                    return "Password";
                case Type.Text:
                    return "Text";
                default:
                    return "What?!";
            }
        }
        public static Type FromString(this Type t1, string s1)
        {
            switch (s1)
            {
                case "Button":
                    return Type.Button;
                case "String":
                    return Type.String;
                case "Number":
                    return Type.Number;
                case "Boolean":
                    return Type.Boolean;
                case "Date":
                    return Type.Date;
                case "Time":
                    return Type.Time;
                case "DateTime":
                    return Type.DateTime;
                case "List":
                    return Type.List;
                case "Dictionary":
                    return Type.Dictionary;
                case "Object":
                    return Type.Object;
                case "File":
                    return Type.File;
                case "Image":
                    return Type.Image;
                case "Video":
                    return Type.Video;
                case "Audio":
                    return Type.Audio;
                case "Email":
                    return Type.Email;
                case "Url":
                    return Type.Url;
                case "Phone":
                    return Type.Phone;
                case "Color":
                    return Type.Color;
                case "Password":
                    return Type.Password;
                case "Text":
                    return Type.Text;
                default:
                    return Type.String;
            }
        }
    }
    public static class StyleMethods
    {
        public static String GetString(this Style s1)
        {
            switch (s1)
            {
                case Style.Property:
                    return "Property";
                case Style.Attribute:
                    return "Attribute";
                default:
                    return "What?!";
            }
        }
        public static Style FromString(this Style s1, string s2)
        {
            switch (s2)
            {
                case "Property":
                    return Style.Property;
                case "Attribute":
                    return Style.Attribute;
                default:
                    return Style.Property;
            }
        }
    }
    internal class Validator
    {
        
        Style style = Style.Property;
        Type type = Type.String;
        Boolean required = true;
        int min_words = 10;
        int max_words = 100;

        public Type InstructionType { get => type; set => type = value; }
        public int Min_Words { get => min_words; set => min_words = value; }
        public int Max_Words { get => max_words; set => max_words = value; }
        public Style InstructionStyle { get => style; set => style = value; }
        public bool Required { get => required; set => required = value; }

        internal static void ValidateInstruction(KDLNode node, StreamWriter sw)
        {
            foreach (KDLNode n in node.Child.Nodes){
                
            }

        }
    }
}