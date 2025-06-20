namespace ACViewer.Entity
{
    public class ClothingTextureEffect
    {
        public ACE.DatLoader.Entity.CloTextureEffect _effect;

        public ClothingTextureEffect(ACE.DatLoader.Entity.CloTextureEffect effect)
        {
            _effect = effect;
        }

        public override string ToString()
        {
            return $"OldTex: {_effect.OldTexture:X8}, NewTex: {_effect.NewTexture:X8}";
        }
    }
}
