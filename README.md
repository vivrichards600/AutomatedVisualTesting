# Automated Visual Testing

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

## About 
The following solution is based off an idea I found here https://www.codeproject.com/articles/374386/simple-image-comparison-in-net to compare images. I have taken this idea further to automate screen capture both to disk and in memory for speed whilst running tests using Selenium WebDriver.

The idea is to create screenshots of your web app to capture what you expect it to look like - these get saved to the Screenshots folder. Each time you want to run regression tests you simply get selenium to go to the web pages you specify (relating to the images we captured) and take a new screenshot (which we hold in memory) and then compare the images to see if they differ. 

The tester can change the acceptable percent of change to allow between images. If tests keep failing when not expected there is the ability to take screenshots on test failure which will then enable you to do some quick comparison of the actual and expected pages.

## Further info

This is a work in progress and the code is just a quick and dirty way to get some automated visual testing done (it needs a bit of work!). Feel free to fork and improve this solution - I'd love any help with this idea.
