using ACViewer.Entity;

namespace ACViewer.FileTypes
{
    public class PaletteSet
    {
        public ACE.DatLoader.FileTypes.PaletteSet _paletteSet;

        public PaletteSet(ACE.DatLoader.FileTypes.PaletteSet paletteSet)
        {
            _paletteSet = paletteSet;
        }

        public TreeNode BuildTree()
        {
            var treeView = new TreeNode($"{_paletteSet.Id:X8}");

            foreach (var paletteID in _paletteSet.PaletteList)
                treeView.Items.Add(new TreeNode($"{paletteID:X8}", clickable: true));

            return treeView;
        }
    }
}
