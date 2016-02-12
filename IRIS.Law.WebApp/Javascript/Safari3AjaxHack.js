// Safari 3 AJAX "issue". It no longer needs JavaScript hacks that are still implemented
// http://forums.asp.net/p/1252014/2392110.aspx
// This javascript added to resolve validation issues in safari or chrome
Sys.Browser.WebKit = {}; //Safari 3 is considered WebKit
if (navigator.userAgent.indexOf('WebKit/') > -1) {
    Sys.Browser.agent = Sys.Browser.WebKit;
    Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
    Sys.Browser.name = 'WebKit';
}