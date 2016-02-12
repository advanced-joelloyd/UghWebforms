/*****************************************************************************************************************/ 
/*Function name :showToolTip(e,text) */ 
/*Usage of this function :Function to show help tooltip on mouse over 
/*Input parameter required:e = passes fired event */ 
/*Input parameter required:text = text reuired for tooltip content*/
/*****************************************************************************************************************/ 
function showToolTip(e) {
    //debugger;
     //Checks for the event.
	if(document.all)
	{
	    e = event;
	}
	var errorMesg;
	if (navigator.userAgent.indexOf("Firefox") > -1) {
	    errorMesg = e.currentTarget.attributes["ErrorMessage"];
	}
	else {
	    errorMesg = e.srcElement.attributes["ErrorMessage"];
	}
	if (errorMesg != undefined && errorMesg != null)
	{
	    if (errorMesg.value != undefined && errorMesg.value != "null") {
			//Gets the bubble tooltip
			var toolTip = document.getElementById('bubble_tooltip');
				
			//Gets the bubble tooltip
			toolTip.innerHTML = errorMesg.value;
			
			//Gets the bubble tooltip
			toolTip.style.display = 'block';
			
			//Gets the scroll for bubble tooltip
			var toolTipScroll  = Math.max(document.body.scrollTop,document.documentElement.scrollTop);
			
			//Gets the left Position for tooltip
			var leftPos = e.clientX;
			
			//Checks the position of bubble tooltip
			if(leftPos<0)
			{
				leftPos = 0;
			}
			
			//Checks the position of bubble tooltip
			toolTip.style.left = leftPos + 'px';
			
			//Checks the position of bubble tooltip
			toolTip.style.top = e.clientY - toolTip.offsetHeight -5 + toolTipScroll  + 'px';
		}
	}
}	

/*****************************************************************************************************************/ 
/*Function name :hideToolTip() */ 
/*Usage of this function :Function to hide help tooltip on mouse out
/*****************************************************************************************************************/ 
function hideToolTip()
{
    //Hides the bubble toolTip on mouse out event.
	document.getElementById('bubble_tooltip').style.display = 'none';	
}

