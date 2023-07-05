truncate `{{table.value_type}}`;
insert `{{table.value_type}}`
{{~column = datas[0]~}}
{{~size = column.fields.size~}}
({{~i = 0~}}{{~for f in column.fields~}}{{~item = column.impl_type.hierarchy_export_fields[i]~}}{{~if item~}}`{{~item.name~}}`{{~if size - 1 > i~}},{{~end~}}{{~end~}}{{~i = i + 1~}}{{~end~}})
value
{{~i = 0~}}{{~j = 0~}}{{~for d in datas~}}{{~dataSize = datas.size~}}({{~for f in d.fields~}}{{~if f ~}}{{~item = d.impl_type.hierarchy_export_fields[i]~}}{{~if item~}}{{f}}{{~if size - 1 > i~}},{{~end~}}{{~end~}}{{~i = i + 1~}}{{~end~}}{{~end~}}){{~if dataSize - 1 > j~}}
,
{{else}};{{~end~}}{{~i = 0~}}{{~j = j + 1~}}{{~end~}}