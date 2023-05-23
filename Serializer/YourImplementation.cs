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
            throw new NotImplementedException();
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
    }
}
