		public async Task<IActionResult> GetAllAsync([FromRoute] int PollId, CancellationToken cancellationToken)
		{
			var poll = await _unitOfWork.polls.GetByIdAsync(PollId);
			if (poll is null)
				return NotFound(PollErrors.NotFound);

			//مفتاح هعمل الكاش بيه وهحذفه وهجيبه بيه _
			//يونيك فاليو علشان ميحفظش كل الاسأله بكل البولز بتاعتها بنفس المفتاح علشان لما تجيب الاسأله بتاعة بول واحد يجيب بتاعة بول واحد بس
			var cachKey = $"{cashPrefix}-{PollId}";
			//Inject this class (not interface)
			var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<Question>>(
				cachKey, async cachEntriy => await _unitOfWork.questions
				.FindAllInclude(x => x.PollId == PollId, cancellationToken, new[] { "answers" })
				);
			var response = questions.Adapt<IEnumerable<QuestionResponse>>();
			await _cashService.SetAsync(cachKey, response, cancellationToken);
			return Ok(response);
		}

// In Create ,Update and Delete endpoint to get last vesion from caching
await _hybridCache.RemoveAsync($"{cashPrefix}-{question.PollId}", cancellationToken);
