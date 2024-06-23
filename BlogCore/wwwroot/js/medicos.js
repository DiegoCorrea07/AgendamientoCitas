@{
    ViewData["Title"] = "Lista de Médicos";
}

<div class="container">
    <h2 class="text-center">Lista de Médicos</h2>
    <hr />

    <table id="tblMedicos" class="table table-bordered table-striped">
        <thead>
            <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Especialidad</th>
                <th>Imagen</th>
                <th>Acciones</th>
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>

@section Scripts {
    <script>
        var dataTable;

        $(document).ready(function () {
            cargarDatatable();
        });

        function cargarDatatable() {
            dataTable = $("#tblMedicos").DataTable({
                "ajax": {
                    "url": "/Admin/Medicos/GetAll",
                    "type": "GET",
                    "datatype": "json"
                },
                "columns": [
                    { "data": "id", "width": "5%" },
                    { "data": "nombre", "width": "30%" },
                    { "data": "especialidad.nombre", "width": "30%" }, // Accede al nombre de la especialidad
                    {
                        "data": "urlImagen",
                        "render": function (data) {
                            return <img src="${data}" class="img-thumbnail" style="width: 100px; height: auto;" />;
                        }, "width": "15%"
                    },
                    {
                        "data": "id",
                        "render": function (data) {
                            return <div class="text-center">
                                <a href="/Admin/Medicos/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                            </a>
                                & nbsp;
                            <a onclick=Delete("/Admin/Articulos/Delete/${data}") class="btn btn-danger text-white" style = "cursor:pointer; width:140px;" >
                                <i class="far fa-trash-alt"></i> Borrar
                                        </a>
                                    </div >;
}, "width": "20%"
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

function eliminarMedico(url) {
    swal({
        title: "¿Está seguro de borrar?",
        text: "¡Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (err) {
                toastr.error("Error al intentar eliminar el médico.");
            }
        });
    });
}

    </script >
}