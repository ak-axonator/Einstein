# from cairosvg import svg2png

# svg_code = """
#     <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
#         <circle cx="12" cy="12" r="10"/>
#         <line x1="12" y1="8" x2="12" y2="12"/>
#         <line x1="12" y1="16" x2="12" y2="16"/>
#     </svg>
# """

# svg2png(bytestring=svg_code,write_to='output.png')


# Set input and output file names
input_file = 'E:\JK\SVG Work\python\HeroSVG_svg_source.svg'
output_file1 = 'E:\JK\SVG Work\python\HeroSVG_png_generated.png'
output_file2 = 'E:\JK\SVG Work\python\HeroSVG_webp_generated.webp'

from svglib.svglib import svg2rlg, register_font, find_font
from reportlab.graphics import renderPM
from reportlab.lib import fonts

font_name = "Montserrat"
register_font(font_name, "E:\JK\SVG Work\python\Montserrat-Bold.ttf", weight='bold', style='normal')

#register_font_family(self, family, normal,  bold=None, italic=None, bolditalic=None):
font = find_font(font_name, weight='bold', style='normal')

drawing = svg2rlg(input_file)

renderPM.drawToFile(drawing, output_file1, fmt="JPG")
from PIL import Image
im = Image.open(output_file1).convert("RGB")
im.save(output_file2, "WEBP")

