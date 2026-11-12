using UnityEngine;

public class TVSwitcher : MonoBehaviour
{
    public Renderer screen;
    public Texture[] pictures;

    public float changeTime = 5;

    private int currPic = 0;
    private float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen.material.mainTexture = pictures[currPic];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= changeTime)
        {
            timer = 0f;
            currPic++;

            if(currPic >= pictures.Length)
            {
                currPic = 0;
            }

            screen.material.mainTexture = pictures[currPic];
        }
    }
}
