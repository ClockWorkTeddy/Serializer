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
            StringBuilder result = new("{");

            while (head != null)
            {
                result.Append(WriteNode(head));
                head = head.Next;
            }

            s.Write(Encoding.Default.GetBytes(result.ToString()));

            return Task.CompletedTask;
        }

        private StringBuilder WriteNode(ListNode node)
        {
            StringBuilder result = new();

            result.Append(node.ToString() + ": {");
            result.Append($"Data: {node.Data},");
            result.Append($"Previous: {node.Previous.ToString()}");
            result.Append($"Random: {node.Random.ToString()}");
            result.Append($"Next: {node.Next.ToString()}");

            return result;
        }
    }
}
