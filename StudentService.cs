
private readonly HybridCache _hybridCache = hybridCache; //Inject
private const string cashPrefix = "allStudent";
public async Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
{
	//مفتاح هعمل الكاش بيه وهحذفه وهجيبه بيه _
	//يونيك فاليو علشان ميحفظش كل الاسأله بكل البولز بتاعتها بنفس المفتاح علشان لما تجيب الاسأله بتاعة بول واحد يجيب بتاعة بول واحد بس
	var cacheKey = cashPrefix;
	var students = await _hybridCache.GetOrCreateAsync<IEnumerable<StudentResponse>>(
		cacheKey,
		async cacheEntry =>
		{
			var studentEntities = await _unitOfWork.Student.FindAllInclude(cancellationToken, ["Department"]);
			return studentEntities.Adapt<IEnumerable<StudentResponse>>();
		});
	return students;
}

// In Create ,Update and Delete endpoint to get last vesion from caching
await _hybridCache.RemoveAsync(cashPrefix, cancellationToken);
