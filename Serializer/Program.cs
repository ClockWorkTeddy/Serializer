using SerializerTests.Implementations;
using SerializerTests.Nodes;

ListNode nodeB = new ListNode() { Data = "B" };
ListNode nodeA = new ListNode() { Data = "A", Next = nodeB };
nodeB.Previous = nodeA;

JohnSmithSerializer serializer = new JohnSmithSerializer();
using (FileStream fStream = new(@"D:\sample.json", FileMode.OpenOrCreate))
{
    serializer.Serialize(nodeA, fStream);
    fStream.Close();
}