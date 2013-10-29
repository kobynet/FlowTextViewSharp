FlowTextViewSharp
=================

A direct port (using sharpen + a bit of manual coding) of [FlowTextView](https://code.google.com/p/android-flowtextview/) to C# to work with [Xamarin.Android](http://xamarin.com/monoforandroid)

Usage
-----

Xml:
    <?xml version="1.0" encoding="utf-8"?>
    <LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
        android:orientation="vertical"
        android:layout_width="fill_parent"
        android:layout_height="fill_parent"
        >
      <com.kobynet.flowtext.FlowTextViewSharp
                   android:id="@+id/tv"
                   android:layout_width="fill_parent"
                   android:layout_height="wrap_content" >
        <ImageView
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_alignParentLeft="true"
                        android:layout_alignParentTop="true"
                        android:padding="10dip"
                        android:src="@drawable/icon" />
        </com.kobynet.flowtext.FlowTextViewSharp>
    </LinearLayout>

Code:
    var tv = (FlowTextViewSharp)FindViewById (Resource.Id.tv);
    ISpanned spannable = Html.FromHtml (@"<p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <p> tklsdflksd blasd sdfsmdklfmsdlkfmlksd f </p> <br> <p> aslkdlamdlkamslkdmaslkdmalksmdlasmdlkmalksdm </p> <br> <p> aslkdlamdlkamslkdmaslkdmalksmdlasmdlkmalksdm </p> <td><strong>Class </strong><a href=""#5A"">5A</a> | <a href=""#4A"">4A</a> | <a href=""#3A"">3A</a> | <a href=""#2A"">2A</a> | <a href=""#1A"">1A</a> | <a href=""#8man"">8-man</a> | <a href=""#6man"">6-man</a></td>");
    tv.Color = Color.White;
    tv.Text = spannable;


Thanks to
---------
* [Dean Wild](http://www.deanwild.co.uk/) - The creator of [FlowTextView](https://code.google.com/p/android-flowtextview/)

TODO
----
* Code Refactoring + cleaning.
* Optimizations.
* Add documentation.

License
-------
Just like the original this port is licensed under Apache 2.0.
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
    http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
