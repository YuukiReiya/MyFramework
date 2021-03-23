namespace Yuki.ManagedPlugins
{
    /// <summary>
    /// 定数宣言
    /// </summary>
    public class Define
    {
        /// <summary>
        /// コンストラクタ隠蔽
        /// </summary>
        private Define() { }
        public const int NOTHING_LAYER_VALUE = 0;
        public const int EVERYTHING_LAYER_VALUE = ~0;
        public const float MAX_ALPHA = 1.0f;
        public const float MIN_ALPHA = 0.0f;
        public const string ANDROID_LOCAL_HEADER = "file:///";
    }
}
