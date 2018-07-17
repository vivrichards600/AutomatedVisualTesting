# Automated Visual Testing

![Differences screenshot](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/diff.png "Chrome Differences Screenshot")

## About this framework

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be and looking as expected! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

It is also necessary sometimes to check contents of files, which can be quite time consuming to open up pdf files to check that the contents are as expected. Using this framework it is also possible to save all pdf pages as images and compare those to previously saved pdf images to check for changes.

## How it works 

### Base images for websites
Once you are happy with how a particular area or how the whole of your web page looks you write tests using this framework. The first time you run your tests the base images will not exist,this framework will automatically take the base images for you and alert you to tell you that this has been done.

### Compare a base image of a web page to an image of a web page taken by visiting a url:

``` c#
[TestMethod]
public void Full_Page_Comparison_Test()
{
	// Arrange
	Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
    	var baseImage = "ComputerDatabaseHomePage.Chrome.png";
    	
	// Act
	var result = Differences(baseImage, Driver, _options);

	// Assert
	Assert.IsTrue(result.Match);
}
```

### Compare a base image of an element on a web page to an image of an element taken by visiting a url:

``` c#
[TestMethod]
public void Individual_Page_Element_Comparison_Test()
{
	// Arrange
	Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
    	var element = ".computers";
    	var baseImage = "ComputerDatabaseTable.Chrome.png";

	// Act
    	var result = Differences(baseImage, element, Driver);

	// Assert
    	Assert.IsTrue(result.Match);
}
```

### Compare a base image of a web page, whilst covering a dynamic element to an image of a web page taken by visiting a url:

``` c#
[TestMethod]
public void Full_Page_Comparison_Covering_Dynamic_Element_Test()
 {
	// Arrange
	Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
    	var baseImage = "ComputerDatabaseHomePageWithoutTable.Chrome.png";
    	
	// Act
	CoverDynamicElementBySelector(Driver, ".computers");
	var result = Differences(baseImage, Driver, _options);

	// Assert
    	Assert.IsTrue(result.Match);
}
```

### Compare pdf page to an Image taken previously from a pdf:

``` c#
[TestMethod]
public void Pdf_Page_One_Comparison_Test()
{
	//Arrange
  	var baseImage = "PDFPage1.png";
    	var actualPageImage = GetPdfPageAsImage("Test.pdf", 1);

	// Act
    	var result = Differences(baseImage, actualPageImage, _options);

	// Assert
    	Assert.IsTrue(result.Match);
}
```
## Debugging when tests fail

When your tets fail because results were not as expected, the framework will take screenshots of what it actually compared as well as an image displaying where the differences were found. The directory where these images are stored are configurable in the app.config. 

## Settings
The app.config contains various settings to enable you to specify:


The path to obtain base images from
``` xml
<add key="TestDataDirectory" value="../../TestData/" />
```

The path to store difference and actual images to
``` xml
<add key="OutputDirectory" value="../../TestData/" />
```   
