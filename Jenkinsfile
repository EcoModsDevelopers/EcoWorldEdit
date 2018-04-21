node {
	stage 'Checkout'
		checkout scm

	stage 'Build'
		bat "\"${tool 'MSBuild'}\" /restore"
		bat "\"${tool 'MSBuild'}\" EcoWorldEdit.sln /p:Configuration=Release /p:Platform=\"Any CPU\" /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"

	stage 'Archive'
		bat "7z a EcoWorldEdit.zip Mods/"
		archiveArtifacts 'EcoWorldEdit.zip'
}