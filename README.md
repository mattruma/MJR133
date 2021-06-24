# Introduction

This sample app displays a list of blobs in a container.

A SAS Uri is generated to access the blob, either by the web app or a logic app.

To use the web app to generate the SAS Uri set the value of `SAS_GENERATION_METHOD` in your app settings to `webapp`, to use the logic app, set it to `logicapp`.