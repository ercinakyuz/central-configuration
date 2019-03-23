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
                slcValue.attr("required", "required");
                txtValue.removeAttr("required");
                txtValue.prop("disabled", true);
                slcValue.prop("disabled",false);
                txtValue.val("");
            } else {

                slcValue.hide();
                txtValue.show();
                txtValue.attr("required", true);
                slcValue.removeAttr("required");
                slcValue.prop("disabled", true);
                txtValue.prop("disabled",false);
                slcValue.val("");
            }
        }
    }
}


