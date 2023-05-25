using SerializerTests.Implementations;
using SerializerTests.Nodes;

ListNode nodeB = new ListNode() { Data = "B" };
ListNode nodeA = new ListNode() { Data = "A", Next = nodeB };
nodeB.Previous = nodeA;

JohnSmithSerializer serializer = new JohnSmithSerializer();

ListNode another = new ListNode();
using (FileStream fStream = new(@"D:\sample.json", FileMode.OpenOrCreate))
{
    serializer.Serialize(nodeA, fStream);
    fStream.Close();
}

using (FileStream fStream = new(@"D:\sample.json", FileMode.Open))
{
    another = await serializer.Deserialize(fStream);
    fStream.Close();
}

using (FileStream fStream = new(@"D:\again.json", FileMode.OpenOrCreate))
{
    serializer.Serialize(another, fStream);
    fStream.Close();
}

ListNode copied = await serializer.DeepCopy(nodeA);

Console.WriteLine(copied);

