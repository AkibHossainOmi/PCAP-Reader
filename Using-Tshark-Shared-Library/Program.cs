using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

class Programs {
    static List<Dictionary<string, JToken>> dictionary_list = new List<Dictionary<string, JToken>>();
    static Dictionary<string, Dictionary<string, JToken>> main_dictionary = new Dictionary<string, Dictionary<string, JToken>>();

    [DllImport("{fullpath}/Tshark-shared-Library/wireshark/build/run/libtshark.so")]
    public static extern IntPtr tb_main(int argc, string[] argv);
    static void Main(string[] args) 
    {
        IntPtr resultPtr =tb_main(args.Length, args);
        string? result;
        result = Marshal.PtrToStringAnsi(resultPtr);
        if(result != null) Adding_To_Dictionary(result);
        foreach(var dictionary in dictionary_list)
        {
            string key = dictionary["mtp3.opc"] + "/" + dictionary["mtp3.dpc"] + "/" + dictionary["mtp3.sls"] + "/" + dictionary["tcap.tid"] + "/" + dictionary["sccp.called.digits"] + "/" + dictionary["sccp.calling.digits"];
            main_dictionary[key] = dictionary;
            foreach(var pair in main_dictionary[key]) 
            {
                Console.WriteLine($"{key}\t->\t{pair.Key}: {pair.Value}");
            }
        }

        Console.WriteLine("\ntask finished");
    }
    static List<KeyValuePair<string, JToken>> ExtractKeysAndValues(JObject jsonObject)
    {
        List<KeyValuePair<string, JToken>> pairs = new List<KeyValuePair<string, JToken>>();
        foreach (var property in jsonObject.Properties())
        {
            string key = property.Name;
            JToken value = property.Value;
            if (value is JObject nestedObject)
            {
                pairs.AddRange(ExtractKeysAndValues(nestedObject));
            }
            else
            {
                pairs.Add(new KeyValuePair<string, JToken>(key, value));
            }
        }
        return pairs;
    }

    static void Adding_To_Dictionary(string result)
    {
        JArray packets = JArray.Parse(result);

        foreach(JObject packet in packets.Cast<JObject>())
        {
            List<KeyValuePair<string, JToken>> pairs = ExtractKeysAndValues(packet);
            Dictionary<string, JToken> dictionary =  new Dictionary<string, JToken>();
            foreach (var pair in pairs)
            {
                if(!dictionary.TryGetValue(pair.Key, out JToken? value)) dictionary.Add(pair.Key,pair.Value);
            }
            dictionary_list.Add(dictionary);
        }
    }
}