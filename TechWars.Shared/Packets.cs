using LiteNetLib.Utils;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TechWars
{    
    // S: Server to Player
    // P: Player to Server

    public enum PacketType : byte
    {
        Serialized = 0,
        Input,
        State,
        PlayerEnteredView,
        PlayerExitedView,
        Ack
    }

    //Auto serializable packets
    
    public class Packet_P_JoinPacket
    {
        public string UserName { get; set; }
    }

    public class Packet_S_JoinAccept
    {
        public byte clientID { get; set; }
        public ushort ServerTick { get; set; }
    }
    public class Packet_S_PlayerJoined
    {
        public int clientID { get; set; }
        public string Username { get; set; }
        public bool NewPlayer { get; set; }
        public ushort ServerTick { get; set; }
        public PlayerState InitialPlayerState { get; set; }
    }
    public class Player_S_DisconnectPacket
    {
        /// <summary> The player that disconnected </summary>
        public byte clientID { get; set; }
    }


    public struct Packet_Player : INetSerializable
    {
        public ushort Id;

        public void Deserialize(NetDataReader reader)
        {
            reader.GetUShort();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
        }
    }
    
    /// <summary>
    /// Player to Server input packet. Holds all inputs until last server ack
    /// </summary>
   /* public struct Packet_P_Input : INetSerializable
    {
        /// <summary>  Last acknowledged server tick </summary>
        public ushort tick_server;
        /// <summary>  Local Simulation tick </summary>
        public ushort tick_player;
        /// <summary> The amout of inputs sent </summary>
        public int count => tick_player - tick_server;
        //
        public PlayerInput input;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(tick_server);
            writer.Put(input);
        }
        public void Deserialize(NetDataReader reader)
        {
            tick_server = reader.GetUShort();
            input = reader.Get<PlayerInput>();
        }

	}*/

   public struct Packet_P_Input : INetSerializable
    {
        /// <summary>  Last acknowledged server tick </summary>
        public ushort tick_server;
        /// <summary>  Local Simulation tick </summary>
        public ushort tick_player;

        /// <summary>
		/// From oldset to newest
        /// Todo: Delta compression
        /// </summary>
        public PlayerInput[] inputs;

        public Packet_P_Input(ushort tick_server, ushort tick_player, PlayerInput[] inputs) : this()
        {
            this.tick_server = tick_server;
            this.tick_player = tick_player;
			this.inputs = inputs;
		}

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(tick_server);
            writer.Put(tick_player);
            writer.Put((ushort)inputs.Length);
            for (int i = 0; i < inputs.Length; i++)
            {
                writer.Put(inputs[i]);
            }
        }
        public void Deserialize(NetDataReader reader)
        {
            tick_server = reader.GetUShort();
            tick_player = reader.GetUShort();
            ushort inputCount = reader.GetUShort();
            inputs = new PlayerInput[inputCount];
            for (int i = 0; i < inputCount; i++)
            {
                inputs[i] = reader.Get<PlayerInput>();
            }
        }
   }

    public struct InputPacket : INetSerializable
    {
        public byte relativeTick;
        public InputType inputType;
        public short inputValue;

        public InputPacket(byte relativeTick, InputType inputType, short inputValue)
        {
            this.relativeTick = relativeTick;
            this.inputType = inputType;
            this.inputValue = inputValue;
        }

        public void Deserialize(NetDataReader reader)
        {
            relativeTick = reader.GetByte();
            inputType = (InputType)reader.GetByte();
            inputValue = reader.GetShort();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(relativeTick);
            writer.Put((byte)inputType);
            writer.Put(inputValue);
        }
    }

    public struct Packet_S_GameState : INetSerializable
    {
        /// <summary> Wick tick the server was on when this packet was sent </summary>
        public ushort tick_server;

        public Packet_S_PlayerState[] playerstates;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(tick_server);
            writer.Put((ushort)playerstates.Length);
            for (int i = 0; i < playerstates.Length; i++)
            {
                writer.Put(playerstates[i]);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            tick_server = reader.GetUShort();
			var playerCount = reader.GetUShort();
            playerstates = new Packet_S_PlayerState[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerstates[i] = reader.Get<Packet_S_PlayerState>();
            }
        }
    }

    public struct Packet_S_PlayerState : INetSerializable
    {
        /// <summary> the player networkID </summary>
        public ushort id_network;
        /// <summary> Last processed tick</summary>
        public ushort tick_last;
        //
        public PlayerState state;
        //
        public PlayerInput input;

		public Packet_S_PlayerState(ushort id_network, ushort tick_last, PlayerState state, PlayerInput input)
		{
			this.id_network = id_network;
			this.tick_last = tick_last;
			this.state = state;
			this.input = input;
		}

		public void Serialize(NetDataWriter writer)
        {
            writer.Put(id_network);
            writer.Put(tick_last);
            writer.Put(state);
            writer.Put(input);
        }

        public void Deserialize(NetDataReader reader)
        {
            id_network  = reader.GetUShort();
            tick_last = reader.GetUShort();
            state = reader.Get<PlayerState>();
            input = reader.Get<PlayerInput>();
        }
    }

    public struct Packet_S_ACK : INetSerializable
    {   
        /// <summary> Wick tick the server was on when this packet was sent </summary>
        public ushort tick_ack;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(tick_ack);
        }

        public void Deserialize(NetDataReader reader)
        {
            tick_ack = reader.GetUShort();
        }
    }


    public enum InputType : byte
    {
        axis_x,
        axis_y,
        rotation,
        shooting
    }

    public struct PlayerInput : INetSerializable, IEquatable<PlayerInput>
    {   
        public short axis_x;
        public short axis_y;
        public ushort rotation;
        public byte shooting;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(axis_x);
            writer.Put(axis_y);
            writer.Put(rotation);
			if(shooting != 0)
				writer.Put(true);
			else 
				writer.Put(false);
        }

        public void Deserialize(NetDataReader reader)
        {
            axis_x = reader.GetShort();
            axis_y = reader.GetShort();
            rotation = reader.GetUShort();
			if (reader.GetBool())
				shooting = 1;
			else
				shooting = 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is PlayerInput input && Equals(input);
        }

        public bool Equals(PlayerInput other)
        {
            return axis_x == other.axis_x &&
                   axis_y == other.axis_y &&
                   rotation == other.rotation &&
                   shooting == other.shooting;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(axis_x, axis_y, rotation, shooting);
        }

        public static bool operator ==(PlayerInput left, PlayerInput right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerInput left, PlayerInput right)
        {
            return !(left == right);
        }

        public PlayerInput(float axis_x, float axis_y, float rotation, bool shooting)
        {
            this.axis_x = (short)((axis_x) * short.MaxValue);
            this.axis_y = (short)((axis_y) * short.MaxValue);
            this.rotation = (ushort)(rotation * ushort.MaxValue);
			this.shooting = shooting ? (byte)1 : (byte)0;
        }


		public override string? ToString()
		{
			return $"AxisX: {axis_x}, AxisY: {axis_y} ";
		}
		/*
         
        public Input(float axis_x, float axis_y, float rotation, bool shooting)
        {
            this.axis_x = axis_x;
            this.axis_y = axis_y;
            this.rotation = rotation;
            this.shooting = shooting;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((short)((axis_x)*short.MaxValue));
            writer.Put((short)((axis_y) * short.MaxValue));
            writer.Put((ushort)(rotation * ushort.MaxValue));
            writer.Put(shooting);
        }

        public void Deserialize(NetDataReader reader)
        {
            axis_x      = ((float)reader.GetShort())/short.MaxValue;
            axis_y      = ((float)reader.GetShort()) / short.MaxValue;
            rotation    = ((float)reader.GetUShort()) / ushort.MaxValue;
            shooting    = reader.GetBool();
        }


         */



	}

    public struct PlayerState : INetSerializable
    {
        public float positionX;
        public float positionY;

		public PlayerState(float positionX, float positionY)
		{
			this.positionX = positionX;
			this.positionY = positionY;
		}

		public void Deserialize(NetDataReader reader)
        {
            positionX = reader.GetFloat();
            positionY = reader.GetFloat();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(positionX);
            writer.Put(positionY);
        }
    }
}