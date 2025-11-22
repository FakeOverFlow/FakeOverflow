import {Component, inject, input, output, signal} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {HotToastService} from '@ngxpert/hot-toast';
import {Spinner} from '@services/spinner';
import {
  FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse,
  PostService
} from '../../../../../../../../packages/fakeoverflow-angular-services';

@Component({
  selector: 'app-view-question-create-answer',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './view-question-create-answer.html',
  styleUrl: './view-question-create-answer.scss'
})
export class ViewQuestionCreateAnswer {

  private readonly toastService = inject(HotToastService);
  private readonly spinnerService = inject(Spinner);
  private readonly postService = inject(PostService);

  protected readonly isCreating = signal(false);
  public post = input.required<FakeoverFlowBackendHttpApiFeaturesPostsViewPostViewPostResponse>();
  public postCreated = output<string>();

  protected readonly formGroup: FormGroup = new FormGroup({
    content: new FormControl('', [Validators.required, Validators.minLength(20)]),
    isInternal: new FormControl(false),
  });

  protected submit() {
    if(this.formGroup.invalid) {
      this.toastService.error("The answer is invalid")
      return;
    }

    this.isCreating.set(true);
    const spinner = this.spinnerService.for("Creating answer...");
    this.postService
      .createAnswer(this.post()?.postId!, this.formGroup.value)
      .subscribe({
        next: (result) => {
          this.isCreating.set(false);
          spinner.release()
          if (result) {
            this.postCreated.emit(result.id!);
            this.toastService.success("Successfully created answer");
          }
        },
        error: (error) => {
          spinner.release()
          this.isCreating.set(false);
          this.toastService.error("Oops, something went wrong");
        }
      })
  }
}
