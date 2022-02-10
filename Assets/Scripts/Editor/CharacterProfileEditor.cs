//Jarrod Ariola
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CharacterProfile)), CanEditMultipleObjects]
public class CharacterProfileEditor : Editor {

    //Loads the character profile portait as the thumbnail
	public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        //CharacterProfile profile = (CharacterProfile)target as CharacterProfile;
        CharacterProfile profile = AssetDatabase.LoadAssetAtPath<CharacterProfile>(assetPath);
        if (profile.portrait != null)
        {
            //Get texture
            Texture2D spritePreview = AssetPreview.GetAssetPreview(profile.portrait); 

            Texture2D preview = new Texture2D(width, height);
            //Returning original texture causes error crash
            EditorUtility.CopySerialized(spritePreview, preview);
            return preview;
        }

        return null;
    }

}
