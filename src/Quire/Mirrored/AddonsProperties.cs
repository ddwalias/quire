namespace Quire;

internal static partial class AddonsProperties
{
    public partial struct AddonsData
    {
        private readonly string _path;
        private readonly string _id;

        public AddonsData(string path, string id)
        {
            _path = path;
            _id = id;
        }

        public string Path => _path;

        public string ID => _id;
    }
}
