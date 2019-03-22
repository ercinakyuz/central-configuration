function initDdl() {
    var ddlType = $("#ddlType");
    $(document).ready(function () {
        controlType();
        $(ddlType).change(function () {
            controlType();
        });
    });

    function controlType() {
        const slcValue = $("#slcValue");
        const txtValue = $("#txtValue");
        if (ddlType) {

            if (ddlType.val() === "bool") {
                slcValue.show();
                txtValue.hide();
                slcValue.prop("required", true);
                txtValue.removeProp("required");
                txtValue.prop("disabled", true);
                slcValue.removeProp("disabled");
                txtValue.val("");
            } else {

                slcValue.hide();
                txtValue.show();
                txtValue.prop("required", true);
                slcValue.removeProp("required");
                slcValue.prop("disabled", true);
                txtValue.removeProp("disabled");
                slcValue.val("");
            }
        }
    }
}


