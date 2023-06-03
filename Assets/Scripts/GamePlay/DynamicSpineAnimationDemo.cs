using Spine.Unity;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DynamicSpineAnimationDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateSpine());
    }

    IEnumerator CreateSpine()
    {
        var texturePath = Path.Combine(Application.streamingAssetsPath, "Res/Skins/LiuYan/01/dynamic/XingXiang.png");

        var request = UnityWebRequestTexture.GetTexture(texturePath);

        yield return request.SendWebRequest();
        
        var texture = DownloadHandlerTexture.GetContent(request);
        texture.name = "XingXiang";

        request = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "Res/Skins/LiuYan/01/dynamic/XingXiang.atlas"));

        yield return request.SendWebRequest();

        var atlas = new TextAsset(request.downloadHandler.text);

        request = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "Res/Skins/LiuYan/01/dynamic/XingXiang.skel"));

        yield return request.SendWebRequest();

        var skeleton = new TextAsset
        {
            name = "XingXiang.skel"
        };

        var material = Resources.Load<Material>("UI/SpineSkeletonPropertySource");
        var saa = SpineAtlasAsset.CreateRuntimeInstance(atlas, new[] { texture }, material, true);
        var sda = SkeletonDataAsset.CreateRuntimeInstance(skeleton, saa, false);
        sda.SetOverwriteBinaryData(request.downloadHandler.data);
        sda.GetSkeletonData(false);

        var sa = SkeletonAnimation.NewSkeletonAnimationGameObject(sda);

        sa.transform.SetParent(transform, false);

        sa.AnimationName = sa.Skeleton.Data.Animations.First()?.Name;
    }
}
