using Spine;
using Spine.Unity;
using UnityEngine;

public class ReplaceSpineAnimationAttachmentDemo : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation m_SkeletonAnimation;
    [SerializeField] private Texture2D m_Texture;

    void Start()
    {
        CreateRegionAttachmentByTexture(m_SkeletonAnimation.skeleton.FindSlot("taijitu2"), m_Texture);
    }

    private static AtlasRegion CreateRegion(Texture texture)
    {
        var region = new AtlasRegion
        {
            width = texture.width,
            height = texture.height,
            originalWidth = texture.width,
            originalHeight = texture.height,
            rotate = false,
            page = new AtlasPage
            {
                name = texture.name,
                width = texture.width,
                height = texture.height,
                uWrap = TextureWrap.ClampToEdge,
                vWrap = TextureWrap.ClampToEdge
            }
        };
        return region;
    }

    public Material CreateRegionAttachmentByTexture(Slot slot, Texture2D texture)
    {
        if (slot?.Attachment is not RegionAttachment oldAtt || texture == null) return null;

        RegionAttachment att = new RegionAttachment(oldAtt.Name)
        {
            RendererObject = CreateRegion(texture),
            Width = oldAtt.Width,
            Height = oldAtt.Height,
            Path = oldAtt.Path,
            X = oldAtt.X,
            Y = oldAtt.Y,
            Rotation = oldAtt.Rotation,
            ScaleX = oldAtt.ScaleX,
            ScaleY = oldAtt.ScaleY
        };

        att.SetUVs(0f, 1f, 1f, 0f, false);
        att.UpdateOffset();

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = texture
        };
        ((AtlasRegion)att.RendererObject).page.rendererObject = mat;

        slot.Attachment = att;
        return mat;
    }

    public Material CreateMeshAttachmentByTexture(Slot slot, Texture2D texture)
    {
        if (slot == null) return null;
        if (slot.Attachment is not MeshAttachment oldAtt || texture == null) return null;

        MeshAttachment att = new MeshAttachment(oldAtt.Name)
        {
            RendererObject = CreateRegion(texture),
            Path = oldAtt.Path,
            Bones = oldAtt.Bones,
            Edges = oldAtt.Edges,
            Triangles = oldAtt.Triangles,
            Vertices = oldAtt.Vertices,
            WorldVerticesLength = oldAtt.WorldVerticesLength,
            HullLength = oldAtt.HullLength,
            RegionRotate = false,
            RegionU = 0f,
            RegionV = 1f,
            RegionU2 = 1f,
            RegionV2 = 0f,
            RegionUVs = oldAtt.RegionUVs
        };

        att.UpdateUVs();

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = texture
        };
        ((AtlasRegion)att.RendererObject).page.rendererObject = mat;

        slot.Attachment = att;
        return null;
    }
}
