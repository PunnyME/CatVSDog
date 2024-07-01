using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
using Google;
using System.Net.Http;
using TMPro;

public class GoogleSignInManager : MonoBehaviour
{
    public string GoogleWebAPI = "825687240091-gfb4osb49fgc7q01p5t2ea3cuoam9tcb.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public TextMeshProUGUI UsernameText;

    public Image UserProfilePic;

    public string imageURL;

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if(task.IsFaulted)
        {
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsyn was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsyn encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;

                UsernameText.text = user.DisplayName;

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
            });
        }
    }

    private string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return imageURL;
    }

    IEnumerator LoadImage(string imageURL)
    {
        WWW www = new WWW(imageURL);
        yield return www;

        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }
}
