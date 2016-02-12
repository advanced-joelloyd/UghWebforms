//function to enable/disable armed forces textbox based on selection
$("#" + chkArmedForces).click(
	  function() {
	      $("#" + txtArmedForcesNo).attr('readonly', !this.checked);
	      if (this.checked) {
	          $("#" + txtArmedForcesNo).removeClass("readonly");
	      }
	      else {
	          $("#" + txtArmedForcesNo).addClass("readonly");
	          $("#" + txtArmedForcesNo).val("");
	      }
	  });

