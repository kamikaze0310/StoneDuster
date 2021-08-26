using ProtoBuf;

namespace StoneDuster
{
    [ProtoContract]
    public class SyncData
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public bool DusterEnabled;
    }
}
