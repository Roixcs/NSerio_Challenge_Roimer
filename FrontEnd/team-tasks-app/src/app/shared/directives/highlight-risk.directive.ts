import { Directive, ElementRef, Input, OnChanges, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appHighlightRisk]',
  standalone: true
})
export class HighlightRiskDirective implements OnChanges {
  @Input() appHighlightRisk: boolean = false;
  @Input() highlightColor: string = '#ffebee';

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  ngOnChanges(): void {
    if (this.appHighlightRisk) {
      this.renderer.setStyle(this.el.nativeElement, 'background-color', this.highlightColor);
      this.renderer.setStyle(this.el.nativeElement, 'border-left', '4px solid #c62828');
    } else {
      this.renderer.removeStyle(this.el.nativeElement, 'background-color');
      this.renderer.removeStyle(this.el.nativeElement, 'border-left');
    }
  }
}
