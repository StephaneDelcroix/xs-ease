# Xamarin Studio Addin for EASE #

This guide describes how to use the Apperian Publisher addin to publish mobile apps to Apperian EASE directly from Xamarin Studio.

This addin is supported by Xamarin Studio version 4.0.0 or higher.

## About the Apperian publisher addin ##
Xamarin Studio is an IDE that allows you create mobile applications in C# for iOS (with Xamarin.iOS) or Android (with Xamarin.Android). The Apperian Publisher addin allows you to publish your mobile applications to Apperian EASE directly from Xamarin Studio.

Once the app is uploaded, an EASE Administrator can manage it through the EASE Portal.

## Install the Apperian Publisher addin ##
To use the Apperian Publisher addin, you first have to install it into Xamarin Studio.

First, open the addin manager

![Addin-in manager menu entry](images/addinmanager0.png)

TO BE COMPLETED

## Publish a Build ##
Now that the addin is installed, you can use it to publish your mobile application.

1. ### Select a mobile project.###
    The current selected project in Xamarin Studio is the one that will be uploaded. So make sure you're on a mobile (Xamarin.iOS or Xamarin.Android) project before you start.

    For iOS project, make sure as well the build platform is set to build for devices, not simulator. For this, either select *AppStore** or **Ad-Hoc** as configuration, or choose a real device as platform.

    ![Project selection](images/projectconfig.png)

    Before you continue, make sure your project builds and has an icon file attached to it (or the upload will fail).

2. ### Choose Publish to Apperian ###
    From the *Project* menu, choose *Publish to Apperian...*
    
    ![Publish to Apperian...](images/publishtoapperian.png)

    At this point, the publish dialog will appear:

    ![Empty publish dialog](images/publishdialogempty.png)

3. ### Pick or Register a publish target ###
    If you already have declared a target, you can pick it from the *Target:* combobox. At this point, the addin will contact the Apperian EASE API, authenticates you, and check if you're doing an update on an existing application, or publishing a new one.

    If you don't have any, you can click on *Register Publish Target...*





