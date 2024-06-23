var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblPacientes").DataTable({
        "ajax": {
            "url": "/Admin/Pacientes/GetAll", 
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "nombre", "width": "15%" },
            { "data": "fechaNacimiento", "width": "10%" },
            { "data": "genero", "width": "10%" },
            { "data": "direccion", "width": "15%" },
            { "data": "celular", "width": "10%" },
            { "data": "email", "width": "15%" },
            {
                "data": "urlImagen",
                "render": function (imagen) {
                    return `<img src="${imagen}" width="120">`;
                },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Pacientes/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                                    <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                <a onclick="eliminarPaciente(${data})" class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                                    <i class="far fa-trash-alt"></i> Borrar
                                </a>
                            </div>`;
                },
                "width": "20%"
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 a 0 de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "width": "100%"
    });
}

function eliminarPaciente(id) {
    swal({
        title: "¿Está seguro de querer borrar?",
        text: "¡Esta acción no se puede deshacer!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true
    }, function () {
        $.ajax({
            type: "DELETE",
            url: "/Admin/Pacientes/Delete/" + id,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (err) {
                console.error(err);
                toastr.error("Error al intentar eliminar el paciente.");
            }
        });
    });
}
