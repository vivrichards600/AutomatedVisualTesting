# Automated Visual Testing

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be! Using this 'Visual' approach tests can execute a lot quicker than simply trying to find each element on a page to ensure things have loaded on to a page.

## About 
The following solution is based off an idea I found here https://www.codeproject.com/articles/374386/simple-image-comparison-in-net to compare images. I have taken this idea further to automate screen capture both to disk and in memory for speed whilst running tests.

The idea is to initially create screenshots of your web app which would be saved to the Screenshots folder, then each time you want to run regression tests you simply get selenium to go to the web pages, take a screenshot and hold it in memory and then compare the images with the ones we stored. The tester can change the acceptable percent of change to allow and also take screenshots on test failure to do some quick comparison.

## Further info

This is a work in progress and the code is just a quick and dirty way to get some automated visual testing done (it needs a bit of work!). Feel free to fork and improve this solution - I'd love any help with this idea.
