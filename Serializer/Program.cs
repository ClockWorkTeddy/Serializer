using SerializerTests.Implementations;
using SerializerTests.Nodes;

ListNode oroginNode = GetComplexList();

JohnSmithSerializer serializer = new JohnSmithSerializer();

ListNode deserializedNode = await SerializeDeserialize(oroginNode);

ListNode copiedNote = await serializer.DeepCopy(deserializedNode);

deserializedNode = null;

Console.WriteLine(copiedNote != null);

async Task<ListNode> SerializeDeserialize(ListNode input)
{
    ListNode another = new ListNode();

    using (FileStream fStream = new(@".\sample.json", FileMode.Create))
    {
        await serializer.Serialize(input, fStream);
        fStream.Close();
    }

    using (FileStream fStream = new(@".\sample.json", FileMode.Open))
    {
        another = await serializer.Deserialize(fStream);
        fStream.Close();
    }

    return another;
}

ListNode GetComplexList()
{
    int nodesQnt = 10;
    Random rnd = new Random();
    Dictionary<int, ListNode> nodes = new();

    for (int i = 0; i < nodesQnt; i++)
        nodes[i] = new ListNode() { Data = i.ToString() };

    for (int i = 0; i < nodesQnt; i++)
    {
        if (i > 0)
            nodes[i].Previous = nodes[i - 1];

        nodes[i].Random = i % 2 == 0 ? nodes[rnd.Next(nodesQnt)] : null;

        if (i < nodesQnt - 1)
            nodes[i].Next = nodes[i + 1];
    }

    return nodes[0];
}
