function doEditMatterPrint(){var a=0,b=0,f=0,m=0,n=0,l="<%=_tcEditMatter.ClientID %>";a="ctl00__cphMain__tcEditMatter__pnlDetails";b="ctl00__cphMain__tcEditMatter__pnlAdditionalInfo";f="ctl00__cphMain__tcEditMatter__pnlPublicFunding";if(a==0||a==""){document.location="editmatter.aspx?PrintPage=true";return}var d="";try{if(document.getElementById("ctl00__cphMain__tcEditMatter__pnlPublicFunding__chkPublicFunded")){var i=document.getElementById("ctl00__cphMain__tcEditMatter__pnlPublicFunding__chkPublicFunded").checked;if(i)d="<br /><br />"+document.getElementById(f).innerHTML}var e="";if(document.getElementById(b)!=null)e="<br /><br />"+document.getElementById(b).innerHTML;var j='<table cellspacing="4" cellpadding="2" width="100%" class="HeaderTab" style="border-bottom:White"><tr><td>&nbsp;</td><td style="width:60px"><input ID="PRINT" type="button" value="Print" onclick="javascript:location.reload(true);window.print();" /></td><td style="width:60px"><input ID="CLOSE" type="button" value="Close" onclick="window.close();" /></td></tr></table>',h=document.getElementById(a).innerHTML;document.getElementById("_printPreview").innerHTML=j+"<div id='myDiv' style='padding:8px; font-size:8pt; font-family:Arial, Helvetica, sans-serif'><table width='100%'><tr><td colspan='2' class='sectionHeader'>Matter Details</td></tr></table>"+document.getElementById("MatterDetails").innerHTML+"<br />"+h+e+d.replace("General Info ","General Info (Public Funding)")+"</div>"}catch(k){}for(var g=document.getElementById("myDiv"),c=0;c<g.childNodes.length;c++)getChildNodes(g.childNodes[c]);document.getElementById("_PageContents").style.display="none"}function doEditClientPrint(){var j=0,i=0,b=0,c=0,f=0;containerID5="ctl00__cphMain__tcEditClient__pnlAddressDetails__ucAddress__tcClientSearch__pnlByUniqueReference__updPnlAdditionalInfo";f="ctl00__cphMain__tcEditClient__pnlAddressDetails__ucAddress__tcClientSearch__pnlAddress__updPnlAddressDetails";b="ctl00__cphMain__tcEditClient__pnlGeneralDetails";c="ctl00__cphMain__tcEditClient__pnlIndividualDetails";containerID6="ctl00__cphMain__tcEditClient__pnlOrganisationDetails";var e="",a="";if(document.getElementById(b)!=null)e=document.getElementById(b).innerHTML;if(document.getElementById(c)!=null)a=document.getElementById(c).innerHTML;else a=document.getElementById(containerID6).innerHTML;var h='<table cellspacing="4" cellpadding="2" width="100%" class="HeaderTab" style="border-bottom:White"><tr><td>&nbsp;</td><td style="width:60px"><INPUT ID="PRINT" type="button" value="Print" onclick="javascript:location.reload(true);window.print();"></td><td style="width:60px"><INPUT ID="CLOSE" type="button" value="Close" onclick="window.close();"></td></tr></table>';document.getElementById("_printPreview").innerHTML=h+"<div id='myDiv' style='padding:8px; font-size:8pt; font-family:Arial, Helvetica, sans-serif'><br /><table width='100%'><tr><td colspan='2' class='sectionHeader'>Address Details</td></tr><tr><td style='vertical-align:top'>"+document.getElementById(f).innerHTML+"</td><td style='vertical-align:top'>"+document.getElementById(containerID5).innerHTML+"</td></tr></table>"+e+"<br /><br />"+a+"</div>";for(var g=document.getElementById("myDiv"),d=0;d<g.childNodes.length;d++)getChildNodes(g.childNodes[d]);document.getElementById("_PageContents").style.display="none"}function getChildNodes(a){if(a.tagName!=null)for(var b=0;b<a.childNodes.length;b++){var e=true,c=a.childNodes[b].tagName,d=document.createElement("div");if(c=="INPUT"||c=="SELECT"||c=="A")a.childNodes[b].disabled=true;if(c=="DIV")if(a.childNodes[b].id=="MapControl"||a.childNodes[b].id=="MapControl2"||a.childNodes[b].id=="Regenerated"||a.childNodes[b].id=="ResetBtnContainer")a.childNodes[b].innerHTML="";if(c=="DIV")if(a.childNodes[b].id=="Regenerate"){a.removeChild(a.childNodes[b]);e=false}if(c=="A"&&a.childNodes[b].className=="link"){d.innerHTML=a.childNodes[b].innerHTML;a.replaceChild(d,a.childNodes[b])}if(c=="INPUT"&&a.childNodes[b].value!="_rdoBtnMatterCompletedYes"&&a.childNodes[b].value!="_rdoBtnMatterCompletedNo"&&a.childNodes[b].type!="hidden"&&a.childNodes[b].type!="image"&&a.childNodes[b].type!="checkbox"){d.innerHTML=a.childNodes[b].value;d.setAttribute("style","font-size:8pt; font-family:arial; vertical-align:bottom;");a.replaceChild(d,a.childNodes[b])}if(c=="INPUT")a.childNodes[b].type=="hidden"&&a.removeChild(a.childNodes[b]);if(c=="INPUT")a.childNodes[b].type=="image"&&a.removeChild(a.childNodes[b]);if(c=="TEXTAREA"){d.innerHTML=a.childNodes[b].value;d.setAttribute("style","border:1px White Solid; font-size:8pt; font-family:arial; color:black");a.replaceChild(d,a.childNodes[b])}if(c=="SELECT")if(a.childNodes[b].selectedIndex!=-1){d.innerHTML=a.childNodes[b].options[a.childNodes[b].selectedIndex].text;d.setAttribute("style","font-size:8pt; font-family:arial; vertical-align:bottom;");a.replaceChild(d,a.childNodes[b])}else a.removeChild(a.childNodes[b]);if(c=="INPUT")if(a.tagName=="SPAN"&&a.className=="checkBox")if(a.childNodes[b].checked==true&&a.childNodes[b].type=="checkbox")a.innerHTML="Yes";else a.innerHTML="No";if(c=="SPAN")if(a.childNodes[b].className=="mandatoryField"&&a.childNodes[b].className!="checkbox")a.childNodes[b].innerHTML="";e&&getChildNodes(a.childNodes[b])}}function getPrint(b){var a=window.open();a.document.writeln("<HTML><HEAD><title>Print Preview</title>");a.document.writeln('<base target="_self"></HEAD>');a.document.writeln('<body MS_POSITIONING="GridLayout" bottomMargin="0"');a.document.writeln(' leftMargin="0" topMargin="0" rightMargin="0">');a.document.writeln('<form method="post">');a.document.writeln("<TABLE width=100%><TR><TD></TD></TR><TR><TD align=right>");a.document.writeln('<INPUT ID="PRINT" type="button" value="Print" ');a.document.writeln('onclick="javascript:location.reload(true);window.print();">');a.document.writeln('<INPUT ID="CLOSE" type="button" value="Close" onclick="window.close();">');a.document.writeln("</TD></TR><TR><TD></TD></TR></TABLE>");a.document.writeln(b);a.document.writeln("</form></body></HTML>")}