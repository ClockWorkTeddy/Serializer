using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations
{
    //Specify your class\file name and complete implementation.
    public class JohnSmithSerializer : IListSerializer
    {
        //the constructor with no parameters is required and no other constructors can be used.
        public JohnSmithSerializer()
        {
            //...
        }

        public Task<ListNode> DeepCopy(ListNode? head)
        {
            Dictionary<ListNode, int> inputDict = new();
            Dictionary<int, ListNode> outputDict = new();

            while (head != null)
            {
                inputDict[head] = head.GetHashCode();
                head = head.Next;
            }

            foreach (var hash in inputDict.Values)
                outputDict[hash] = new ListNode();

            foreach (var key in inputDict.Keys)
                ProcessNode(inputDict, outputDict, key);

            return Task.FromResult(outputDict.Values.First());
        }

        public Task<ListNode> Deserialize(Stream s)
        {
            byte[] bytes = new byte[s.Length];
            s.Read(bytes);

            string jsonBody = Encoding.UTF8.GetString(bytes);

            return Task.FromResult(GetListNodeFromJson(jsonBody));
        }

        public Task Serialize(ListNode? head, Stream s)
        {
            StringBuilder jsonBody = new();

            while (head != null)
            {
                if (head.Previous != null)
                    jsonBody.Append(",\r\n");

                jsonBody.Append(WriteNode(head));

                head = head.Next;
            }

            jsonBody = AddBrackets(jsonBody);

            s.Write(Encoding.Default.GetBytes(jsonBody.ToString()));

            return Task.CompletedTask;
        }

        #region DEEPCOPY

        private static void ProcessNode(Dictionary<ListNode, int> inputDict, 
                                        Dictionary<int, ListNode> outputDict, ListNode key)
        {
            var hashSelf = inputDict[key];
            var hashPrev = key.Previous == null ? 0 : inputDict[key.Previous];
            var hashRand = key.Random == null ? 0 : inputDict[key.Random];
            var hashNext = key.Next == null ? 0 : inputDict[key.Next];

            outputDict[hashSelf].Data = key.Data.Clone().ToString();
            outputDict[hashSelf].Previous = hashPrev == 0 ? null : outputDict[hashPrev];
            outputDict[hashSelf].Random = hashRand == 0 ? null : outputDict[hashRand];
            outputDict[hashSelf].Next = hashNext == 0 ? null : outputDict[hashNext];
        }

        #endregion

        #region SERIALIZE

        private StringBuilder AddBrackets(StringBuilder jsonBody)
        {
            StringBuilder result = new StringBuilder("{\r\n");
            result.Append(jsonBody);
            result.Append("\r\n}");

            return result;
        }

        private StringBuilder WriteNode(ListNode node)
        {
            StringBuilder result = new();

            string strResult = $$"""
            "{{GetHashOrHull(node)}}": {
                "Data": "{{node.Data}}",
                "Previous": "{{GetHashOrHull(node.Previous)}}",
                "Random": "{{GetHashOrHull(node.Random)}}",
                "Next": "{{GetHashOrHull(node.Next)}}"
            }
            """;

            result.Append(strResult);

            return result;
        }

        private string GetHashOrHull(ListNode? node) =>
            node == null ? "null" : node.GetHashCode().ToString();

        #endregion

        #region DESERIALIZE

        private ListNode GetListNodeFromJson(string jsonBody)
        {
            var jsonNodes = GetJsonNodes(jsonBody);
            var dictJsons = GetDictionaryFromJson(jsonNodes);
            var dictNodes = GetDictionaryNodes(dictJsons);

            return LinkNodes(dictNodes, dictJsons);
        }

        private ListNode LinkNodes(Dictionary<string, ListNode> dictNodes, 
                                   Dictionary<string, string> dictJsons)
        {
            foreach (var dictJson in dictJsons) 
            {
                var previous = GetDataFromJson("Previous", dictJson.Value);
                var random = GetDataFromJson("Random", dictJson.Value);
                var next = GetDataFromJson("Next", dictJson.Value);

                dictNodes[dictJson.Key].Previous = previous == "null" ? null : dictNodes[previous];
                dictNodes[dictJson.Key].Random = random == "null" ? null : dictNodes[random];
                dictNodes[dictJson.Key].Next = next == "null" ? null : dictNodes[next];
            }

            return dictNodes.Values.First();
        }

        private Dictionary<string, ListNode> GetDictionaryNodes(Dictionary<string, string> dictJsons)
        {
            Dictionary<string, ListNode> dictNodes = new Dictionary<string, ListNode>();

            foreach (var jsonPair in dictJsons)
            {
                dictNodes[jsonPair.Key] = new ListNode()
                {
                    Data = GetDataFromJson("Data", jsonPair.Value)
                };
            }

            return dictNodes;
        }

        private string GetDataFromJson(string key, string jsonValue)
        {
            var jsonParameters = jsonValue.Split(",");
            var dataParameter = jsonParameters.FirstOrDefault(_ => _.IndexOf(key) != -1)?.Trim();

            return dataParameter.Split(":")[1].Trim();
        }

        private Dictionary<string, string> GetDictionaryFromJson(string[] jsonNodes)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (var jsonNode in jsonNodes)
            {
                var nodeParts = jsonNode.Split(":{");
                dictionary[nodeParts[0]] = nodeParts[1];
            }

            return dictionary;
        }

        private string[] GetJsonNodes(string jsonBody)
        {
            var jsonPrepared = ClearFormatting(jsonBody);

            return jsonPrepared.Split("},");
        }

        private string ClearFormatting(string jsonBody)
        {
            jsonBody = jsonBody.Replace("\r", "");
            jsonBody = jsonBody.Replace("\n", "");
            jsonBody = jsonBody.Replace("\"", "");
            jsonBody = jsonBody.Replace(" ", "");
            jsonBody = jsonBody.Trim();

            int startIndex = jsonBody.IndexOf('{') + 1;
            int endIndex = jsonBody.LastIndexOf("}") - 1;

            return jsonBody.Substring(startIndex, endIndex - startIndex);
        }

        #endregion
    }
}
