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

        public Task<ListNode> DeepCopy(ListNode head)
        {
            throw new NotImplementedException();
        }

        public Task<ListNode> Deserialize(Stream s)
        {
            byte[] bytes = new byte[s.Length];
            s.Read(bytes);

            string jsonBody = Encoding.UTF8.GetString(bytes);

            return Task.FromResult(GetListNodeFromJson(jsonBody));
        }

        public Task Serialize(ListNode head, Stream s)
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

        private string GetHashOrHull(ListNode node) =>
            node == null ? "null" : node.GetHashCode().ToString();

        #endregion

        #region DESERIALIZE

        private ListNode GetListNodeFromJson(string jsonBody)
        {
            var jsonNodes = GetJsonNodes(jsonBody);
            var dictJsons = GetDictionaryFromJson(jsonNodes);
            var dictNodes = GetDictionaryNodes(dictJsons);

            return LinkNodes(dictNodes);
        }

        private ListNode LinkNodes(object dictNodes)
        {
            throw new NotImplementedException();
        }

        private object GetDictionaryNodes(Dictionary<string, string> dictJsons)
        {
            Dictionary<string, ListNode> dictNodes = new Dictionary<string, ListNode>();

            foreach (var jsonPair in dictJsons)
            {
                dictNodes[jsonPair.Key] = new ListNode()
                {
                    Data = GetDataFromJson(jsonPair.Value)
                };
            }

            return dictNodes;
        }

        private string GetDataFromJson(string jsonValue)
        {
            var jsonParameters = jsonValue.Split(",");
            var dataParameter = jsonParameters.FirstOrDefault(_ => _.IndexOf("Data") != -1)?.Trim();

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
