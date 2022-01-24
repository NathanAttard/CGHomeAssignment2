using Firebase.Extensions;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDownloads : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageReference = storage.GetReferenceFromUrl("gs://rockpaperscissors-323d9.appspot.com/");

        StorageReference allBGs = storageReference.Child("Backgrounds");

        int backgroundNo = Random.Range(1, 4);
        Debug.Log("RANDOM INT:" + backgroundNo);

        DownloadBackground(allBGs.Child(backgroundNo + ".jpg"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DownloadBackground(StorageReference image)
    {
        const long maxAllowedSize = 3 * 1024 * 1024;

        image.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
            }
            else
            {
                byte[] fileContent = task.Result;

                Texture2D texture = new Texture2D(10, 10);
                texture.LoadImage(fileContent);

                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                newSprite.name = image.Name;

                CreateBG(newSprite);
            }
        });
    }

    private void CreateBG(Sprite sprite)
    {
        GameObject background = new GameObject();

        DontDestroyOnLoad(background);

        background.name = "Background";
        background.transform.position = Vector3.zero;

        background.AddComponent<SpriteRenderer>();
        background.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
