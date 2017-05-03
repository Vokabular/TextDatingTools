(:~
: @author Boris Lehečka
:)
xquery version "3.0" encoding "utf-8";
let $doc := doc('StcS_Pamatky.xml')
let $doby := $doc//pramen/@dobaVzniku
let $doby := for $doba in distinct-values($doby)
order by $doba return $doba
return
<items count="{count($doby)}"> {
for $doba in $doby
	let $items := $doc//pramen[@dobaVzniku = $doba]
return <item count="{count($items)}">{$doba}</item>
}
</items>