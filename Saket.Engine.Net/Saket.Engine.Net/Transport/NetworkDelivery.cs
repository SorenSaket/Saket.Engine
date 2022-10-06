namespace Saket.Engine.Net
{
    /// <summary>
    /// Delivery methods
    /// </summary>
    public enum NetworkDelivery
    {
        /// <summary>
        /// Unreliable message
        /// </summary>
        Unreliable = 0,

        /// <summary>
        /// Unreliable with sequencing
        /// </summary>
        UnreliableSequenced = 1,

        /// <summary>
        /// Reliable message
        /// </summary>
        Reliable = 3,

        /// <summary>
        /// Reliable message where messages are guaranteed to be in the right order
        /// </summary>
        ReliableSequenced = 4,

        /// <summary>
        /// A reliable message with guaranteed order with fragmentation support
        /// </summary>
        ReliableFragmentedSequenced = 5
    }
}
