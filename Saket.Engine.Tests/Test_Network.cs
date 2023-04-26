using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saket.ECS;
using Saket.Engine.Net;
using Saket.Engine.Net.Snapshotting;
using Saket.Engine.Net.Snapshotting.A;
using Saket.Serialization;
using System;
using System.Numerics;

namespace Saket.Engine.Tests
{
	[TestClass]
	public class Test_Network
	{
		[TestMethod]
		public void Test_Network_Rollback_DropBefore()
		{
			
		}


		[TestMethod]
		public void Test_Network_SeqDiff()
		{
			
		}


        struct NetworkedData : IEquatable<NetworkedData>
        {
            int valueAlpha;
            float valueBeta;

            public NetworkedData(int valueAlpha, float valueBeta)
            {
                this.valueAlpha = valueAlpha;
                this.valueBeta = valueBeta;
            }

            public override bool Equals(object? obj)
            {
                return obj is NetworkedData data && Equals(data);
            }

            public bool Equals(NetworkedData other)
            {
                return valueAlpha == other.valueAlpha &&
                       valueBeta == other.valueBeta;
            }

            public static bool operator ==(NetworkedData left, NetworkedData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(NetworkedData left, NetworkedData right)
            {
                return !(left == right);
            }
        }


        Schema schema = new Schema(new Schema.UserNetworkedObject[]
        {
                // Player
                new ( new[]{ typeof(NetworkedData) }, (e)=>{
                    e.Add(new NetworkedData());
                }, null)
        });

        [TestMethod]
        public void Test_Network_Snapshot_A_Write()
        {
            var writer = TestSnapshot();

            // 4 : entity count
            // 4 : IDNET
            // 2 : Networked Entity Type
            // 8 : NetworkedData
            Assert.AreEqual(4 + 4+2 + 8, writer.Count);
        }

        [TestMethod]
        public void Test_Network_Snapshot_A_Read()
        {
            ByteWriter writer = TestSnapshot();
            var snapshot = new Snapshot_A();

            Snapshot_A_Client.ReadSnapShotA(snapshot, writer.Data, schema);

            Assert.AreEqual((uint)1, snapshot.numberOfEntities);
            Assert.AreEqual((ushort)0, snapshot.objects[0].id_objectType);
            Assert.AreEqual((IDNet)0, snapshot.objects[0].id_network);
        }

        [TestMethod]
        public void Test_Network_Snapshot_A_Apply()
        {
            ByteWriter writer = TestSnapshot();
            var snapshot = new Snapshot_A();

            World world = new();

            Snapshot_A_Client.ReadSnapShotA(snapshot, writer.Data, schema);
            Snapshot_A_Client.ApplySnapshot(world, snapshot, schema);
            Snapshot_A_Client.InterpolateSnapshot(world, 1f, schema, new Snapshot_A(), snapshot, writer);


            var player = world.GetEntity(new ECSPointer(0, 0));

            var acutal = player.Get<NetworkedData>();

            Assert.AreEqual(new NetworkedData(124, 0.412353f), acutal);

        }


        ByteWriter TestSnapshot()
        {
            ByteWriter writer = new();

            World world = new World();
            var e = world.CreateEntity();

            e.Add(new NetworkedEntity(new IDNet(0), 0));
            e.Add(new NetworkedData(124, 0.412353f));

            Snapshot_A_Server.WriteSnapShot(writer, world, schema, out int sizeInBytes);

            return writer;
        }


    }
}